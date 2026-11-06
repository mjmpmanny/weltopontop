using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ObjectSelectionCursors : MonoBehaviour
{
    [SerializeField] private Image cursorTemplate;
    [SerializeField] private float cursorSpeed = 450f;
    
    [SerializeField] private TMP_Text selectionTitle;
    
    [SerializeField] private ObjectDefinition[] slotDefinitions;

    [SerializeField] private Transform previewStage;

    private RectTransform[] previewClaimAreas;
    private Transform[] previewAnchors;
    
    [SerializeField] private float previewFitSize = 1.25f;
    
    [SerializeField] private Camera previewCamera;
    [SerializeField] private RectTransform previewImageRect;

    private readonly Dictionary<int, int> claimedSlotByPlayer = new();
    
    public bool AllPlayersHaveClaimed => cursors.Count > 0 && claimedSlotByPlayer.Count == cursors.Count;

    private readonly List<(PlayerInput player, InputAction move, Mouse mouse, Gamepad gamepad, Image cursor)> cursors = new();

    private static readonly Color[] CursorColors =
    {
        Color.cyan, Color.yellow, Color.magenta, Color.green
    };

    private void OnEnable()
    {
        claimedSlotByPlayer.Clear();
        selectionTitle.text = "Object Selection";
        CreatePreviewSlots();
        RefreshPreviews();
        
        foreach (PlayerInput player in PlayerInput.all)
        {
            int slot = cursors.Count;
            Image cursor = Instantiate(cursorTemplate, transform);
            cursor.name = $"Player {player.playerIndex + 1} Selection Cursor";
            cursor.color = CursorColors[player.playerIndex % CursorColors.Length];
            TMP_Text cursorLabel = cursor.GetComponentInChildren<TMP_Text>(true);
            cursorLabel.text = $"P{player.playerIndex + 1}";
            Rect panel = ((RectTransform)transform).rect;
            Vector2 cursorHalfSize = cursor.rectTransform.rect.size * 0.5f;
            const float cornerMargin = 40f;

            float left = panel.xMin + cursorHalfSize.x + cornerMargin;
            float right = panel.xMax - cursorHalfSize.x - cornerMargin;
            float top = panel.yMax - cursorHalfSize.y - cornerMargin;
            float bottom = panel.yMin + cursorHalfSize.y + cornerMargin;

            cursor.rectTransform.anchoredPosition = player.playerIndex switch
            {
                0 => new Vector2(left, top),
                1 => new Vector2(right, top),
                2 => new Vector2(left, bottom),
                _ => new Vector2(right, bottom)
            };
            
            cursor.gameObject.SetActive(true);

            Mouse pairedMouse = null;
            Gamepad pairedGamepad = null;

            foreach (InputDevice device in player.devices)
            {
                if (device is Mouse mouse)
                    pairedMouse = mouse;

                if (device is Gamepad gamepad)
                    pairedGamepad = gamepad;
            }
            
            cursors.Add((player, player.actions["Move"], pairedMouse, pairedGamepad, cursor));
        }
    }
    
    private void RefreshPreviews()
    {
        for (int slotIndex = 0; slotIndex < slotDefinitions.Length; slotIndex++)
        {
            GameObject prefab = slotDefinitions[slotIndex].Prefab;
            Transform anchor = previewAnchors[slotIndex];

            GameObject frame = new GameObject("Generated Preview");
            frame.transform.SetParent(anchor, false);

            Transform model = CopyPreviewVisuals(prefab.transform, frame.transform);
            model.localPosition = Vector3.zero;
            model.localRotation = Quaternion.identity;

            MeshRenderer[] renderers = frame.GetComponentsInChildren<MeshRenderer>();
            if (renderers.Length == 0)
                continue;

            Bounds bounds = renderers[0].bounds;
            foreach (MeshRenderer renderer in renderers)
                bounds.Encapsulate(renderer.bounds);

            float diameter = bounds.size.magnitude;
            if (diameter <= 0f)
                continue;

            model.position += frame.transform.position - bounds.center;

            float slotFraction = previewClaimAreas[slotIndex].rect.height /
                                 previewImageRect.rect.height;

            float cameraHeight = Vector3.Distance(
                previewCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, 4f)),
                previewCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 4f)));

            float targetSize = cameraHeight * slotFraction * 0.8f;
            frame.transform.localScale = Vector3.one * (targetSize / diameter);
        }
    }
    
    private Transform CopyPreviewVisuals(Transform source, Transform parent)
    {
        Transform copy = new GameObject(source.name).transform;
        copy.SetParent(parent, false);
        copy.localPosition = source.localPosition;
        copy.localRotation = source.localRotation;
        copy.localScale = source.localScale;

        if (source.TryGetComponent(out MeshFilter sourceMesh) &&
            source.TryGetComponent(out MeshRenderer sourceRenderer) &&
            sourceMesh.sharedMesh != null && sourceRenderer.enabled)
        {
            copy.gameObject.AddComponent<MeshFilter>().sharedMesh = sourceMesh.sharedMesh;
            copy.gameObject.AddComponent<MeshRenderer>().sharedMaterials =
                sourceRenderer.sharedMaterials;
        }

        foreach (Transform child in source)
        {
            if (child.gameObject.activeSelf)
                CopyPreviewVisuals(child, copy);
        }

        return copy;
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        RectTransform panelTransform = (RectTransform)transform;
        Rect panel = panelTransform.rect;

        foreach (var (player, move, mouse, gamepad, cursor) in cursors)
        {
            RectTransform cursorRect = cursor.rectTransform;
            Vector2 position = cursorRect.anchoredPosition;

            if (mouse != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    panelTransform, mouse.position.ReadValue(), null, out position);
            }
            else
            {
                position += move.ReadValue<Vector2>() * (cursorSpeed * Time.deltaTime);
            }

            Vector2 limit = (panel.size - cursorRect.rect.size) * 0.5f;
            position.x = Mathf.Clamp(position.x, -limit.x, limit.x);
            position.y = Mathf.Clamp(position.y, -limit.y, limit.y);
            cursorRect.anchoredPosition = position;
            
            bool pressedClaim = (mouse != null && mouse.leftButton.wasPressedThisFrame)
                                || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame);

            if (!pressedClaim || claimedSlotByPlayer.ContainsKey(player.playerIndex))
                continue;

            Vector2 cursorScreenPoint = RectTransformUtility.WorldToScreenPoint(
                null, cursorRect.position);

            for (int slotIndex = 0; slotIndex < previewClaimAreas.Length; slotIndex++)
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(
                        previewClaimAreas[slotIndex], cursorScreenPoint, null))
                    continue;

                if (claimedSlotByPlayer.ContainsValue(slotIndex))
                    break;

                claimedSlotByPlayer.Add(player.playerIndex, slotIndex);
                ShowClaimMarker(slotIndex, player.playerIndex);
                selectionTitle.text =
                    $"Player {player.playerIndex + 1} claimed object {slotIndex + 1}" +
                    $"\n{claimedSlotByPlayer.Count}/{cursors.Count} players ready";
                break;
            }
        }
    }
    
    private void ShowClaimMarker(int slotIndex, int playerIndex)
    {
        GameObject marker = new GameObject("Claim Marker", typeof(RectTransform));
        RectTransform markerRect = (RectTransform)marker.transform;
        markerRect.SetParent(previewClaimAreas[slotIndex], false);

        markerRect.anchorMin = new Vector2(0f, 0f);
        markerRect.anchorMax = new Vector2(1f, 0f);
        markerRect.pivot = new Vector2(0.5f, 0f);
        markerRect.anchoredPosition = Vector2.zero;
        markerRect.sizeDelta = new Vector2(0f, 40f);

        TextMeshProUGUI label = marker.AddComponent<TextMeshProUGUI>();
        label.font = selectionTitle.font;
        label.text = $"P{playerIndex + 1} CLAIMED";
        label.fontSize = 26f;
        label.alignment = TextAlignmentOptions.Center;
        label.color = CursorColors[playerIndex % CursorColors.Length];
        label.raycastTarget = false;
    }
    
    public void AssignUnclaimedPlayers()
    {
        var availableSlots = new List<int>();

        for (int slotIndex = 0; slotIndex < previewClaimAreas.Length; slotIndex++)
        {
            if (!claimedSlotByPlayer.ContainsValue(slotIndex))
                availableSlots.Add(slotIndex);
        }

        foreach (var (player, _, _, _, _) in cursors)
        {
            if (claimedSlotByPlayer.ContainsKey(player.playerIndex))
                continue;

            if (availableSlots.Count == 0)
                break;

            int randomIndex = Random.Range(0, availableSlots.Count);
            claimedSlotByPlayer.Add(player.playerIndex, availableSlots[randomIndex]);
            availableSlots.RemoveAt(randomIndex);
        }
    }

    public GameObject GetClaimedPrefab(int playerIndex)
    {
        if (!claimedSlotByPlayer.TryGetValue(playerIndex, out int slotIndex) ||
            slotIndex >= slotDefinitions.Length ||
            slotDefinitions[slotIndex] == null)
            return null;

        return slotDefinitions[slotIndex].Prefab;
    }
    
    private void CreatePreviewSlots()
    {
        int count = slotDefinitions.Length;
        previewClaimAreas = new RectTransform[count];
        previewAnchors = new Transform[count];

        if (count == 0)
            return;

        Rect image = previewImageRect.rect;
        Rect safeArea = Rect.MinMaxRect(
            image.xMin + 120f, image.yMin + 120f,
            image.xMax - 120f, image.yMax - 200f);

        int columns = Mathf.Min(count, 2);
        int rows = Mathf.CeilToInt((float)count / columns);
        float cellWidth = safeArea.width / columns;
        float cellHeight = safeArea.height / rows;
        float hitSize = Mathf.Min(360f, Mathf.Min(cellWidth, cellHeight) - 16f);

        var availableCells = new List<int>();
        for (int i = 0; i < count; i++)
            availableCells.Add(i);

        for (int slotIndex = 0; slotIndex < count; slotIndex++)
        {
            int randomIndex = Random.Range(0, availableCells.Count);
            int cellIndex = availableCells[randomIndex];
            availableCells.RemoveAt(randomIndex);

            int column = cellIndex % columns;
            int row = cellIndex / columns;
            float wiggleX = (cellWidth - hitSize) * 0.5f - 8f;
            float wiggleY = (cellHeight - hitSize) * 0.5f - 8f;

            Vector2 center = new Vector2(
                safeArea.xMin + (column + 0.5f) * cellWidth +
                    Random.Range(-wiggleX, wiggleX),
                safeArea.yMin + (row + 0.5f) * cellHeight +
                    Random.Range(-wiggleY, wiggleY));

            GameObject claimArea = new GameObject(
                $"Claim Area {slotIndex + 1}", typeof(RectTransform));
            RectTransform claimRect = (RectTransform)claimArea.transform;
            claimRect.SetParent(previewImageRect, false);
            claimRect.anchorMin = claimRect.anchorMax = new Vector2(0.5f, 0.5f);
            claimRect.sizeDelta = Vector2.one * hitSize;
            claimRect.anchoredPosition = center;
            previewClaimAreas[slotIndex] = claimRect;

            Transform anchor = new GameObject($"Preview Anchor {slotIndex + 1}").transform;
            anchor.SetParent(previewStage, false);
            anchor.position = previewCamera.ViewportToWorldPoint(new Vector3(
                Mathf.InverseLerp(image.xMin, image.xMax, center.x),
                Mathf.InverseLerp(image.yMin, image.yMax, center.y),
                4f));
            anchor.rotation = previewCamera.transform.rotation *
                Quaternion.Euler(20f, 35f, 0f);
            previewAnchors[slotIndex] = anchor;
        }
    }

    private void OnDisable()
    {
        foreach (var (_, _, _, _, cursor) in cursors)
            Destroy(cursor.gameObject);

        cursors.Clear();
        
        foreach (Transform anchor in previewAnchors)
            Destroy(anchor.gameObject);

        foreach (RectTransform claimArea in previewClaimAreas)
            Destroy(claimArea.gameObject);
    }
}