using UnityEngine;
using UnityEngine.InputSystem;

public class AutoJoinFirstPlayer : MonoBehaviour
{
    private void Start()
    {
        PlayerInputManager manager = PlayerInputManager.instance;

        if (manager == null)
        {
            Debug.LogError("PlayerInputManager not found!");
            return;
        }

        // Only players registered in the lobby may join.
        manager.DisableJoining();

        if (PlayerInput.all.Count > 0)
            return;

        int playerIndex = 0;

        foreach (InputDevice device in MenuPlayerSelection.SelectedDevices)
        {
            if (device == null || !device.added)
                continue;

            if (device is Keyboard)
            {
                if (Mouse.current != null)
                {
                    manager.JoinPlayer(
                        playerIndex: playerIndex,
                        controlScheme: "Keyboard&Mouse",
                        pairWithDevices: new InputDevice[]
                        {
                            Keyboard.current,
                            Mouse.current
                        }
                    );
                }
                else
                {
                    manager.JoinPlayer(
                        playerIndex: playerIndex,
                        pairWithDevice: device
                    );
                }
            }
            else if (device is Gamepad)
            {
                manager.JoinPlayer(
                    playerIndex: playerIndex,
                    pairWithDevice: device
                );
            }

            playerIndex++;
        }

        MenuPlayerSelection.Clear();
    }
}