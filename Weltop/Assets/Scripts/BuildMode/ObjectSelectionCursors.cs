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
    
    [SerializeField] private RectTransform[] previewClaimAreas;

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

    private void OnDisable()
    {
        foreach (var (_, _, _, _, cursor) in cursors)
            Destroy(cursor.gameObject);

        cursors.Clear();
    }
}