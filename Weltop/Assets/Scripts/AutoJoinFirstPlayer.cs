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

        if (PlayerInput.all.Count > 0)
            return;

        InputDevice device = MenuPlayerSelection.SelectedDevice;

        if (device != null && device.added)
        {
            if (device is Keyboard)
            {
                if (Mouse.current != null)
                {
                    manager.JoinPlayer(
                        playerIndex: 0,
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
                        playerIndex: 0,
                        pairWithDevice: device
                    );
                }
            }
            else
            {
                manager.JoinPlayer(
                    playerIndex: 0,
                    pairWithDevice: device
                );
            }
        }
        else
        {
            Debug.LogWarning("No valid menu device was found.");
        }

        MenuPlayerSelection.SelectedDevice = null;
    }
}