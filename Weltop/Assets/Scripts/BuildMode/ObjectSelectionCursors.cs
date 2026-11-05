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
            cursor.color = CursorColors[slot % CursorColors.Length];
            cursor.rectTransform.anchoredPosition =
                new Vector2((slot - (PlayerInput.all.Count - 1) * 0.5f) * 60f, -160f);
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

            float slotScale = previewClaimAreas[slotIndex].rect.width / 100f;
            frame.transform.localScale =
                Vector3.one * (previewFitSize * slotScale / diameter);
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
                selectionTitle.text =
                    $"Player {player.playerIndex + 1} claimed object {slotIndex + 1}" +
                    $"\n{claimedSlotByPlayer.Count}/{cursors.Count} players ready";
                break;
            }
        }
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
            image.xMin + 20f, image.yMin + 20f,
            image.xMax - 20f, 70f);

        int columns = Mathf.Min(count, 4);
        int rows = Mathf.CeilToInt((float)count / columns);
        float cellWidth = safeArea.width / columns;
        float cellHeight = safeArea.height / rows;
        float hitSize = Mathf.Min(100f, Mathf.Min(cellWidth, cellHeight) - 16f);

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