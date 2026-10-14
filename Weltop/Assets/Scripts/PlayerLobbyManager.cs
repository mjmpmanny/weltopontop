
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerLobbyManager : MonoBehaviour
{
    [SerializeField] private TMP_Text[] playerStatusTexts;
    [SerializeField] private GameObject lobbyPanel;

    private readonly List<InputDevice> joinedDevices = new();
    private readonly List<bool> playerReadyStates = new();
    private IDisposable buttonSubscription;

    private void OnEnable()
    {
        buttonSubscription = InputSystem.onAnyButtonPress.Subscribe(
            new ButtonPressObserver(OnButtonPressed)
        );
    }

    private void OnDisable()
    {
        buttonSubscription?.Dispose();
        buttonSubscription = null;
    }
    
    public void ResetLobby()
    {
        joinedDevices.Clear();
        playerReadyStates.Clear();

        foreach (TMP_Text statusText in playerStatusTexts)
        {
            if (statusText != null)
                statusText.text = "Waiting...";
        }
    }

    
    private void OnButtonPressed(InputControl control)
    {
        if (lobbyPanel == null || !lobbyPanel.activeInHierarchy)
            return;

        InputDevice device = control.device;

        // Treat keyboard and mouse as one player.
        if (device is Mouse)
            device = Keyboard.current;

        if (device == null)
            return;

        int existingPlayerIndex = joinedDevices.IndexOf(device);

        // An already joined player can toggle ready.
        if (existingPlayerIndex >= 0)
        {
            bool readyPressed =
                (device is Keyboard && control == Keyboard.current.enterKey) ||
                (device is Gamepad && control == ((Gamepad)device).buttonSouth);

            if (readyPressed)
            {
                playerReadyStates[existingPlayerIndex] =
                    !playerReadyStates[existingPlayerIndex];

                UpdatePlayerStatus(existingPlayerIndex);
            }

            return;
        }

        // Register a new player.
        if (joinedDevices.Count >= 4)
            return;

        if (device is not Keyboard && device is not Gamepad)
            return;

        int playerIndex = joinedDevices.Count;

        joinedDevices.Add(device);
        playerReadyStates.Add(false);

        UpdatePlayerStatus(playerIndex);

        Debug.Log($"Player {playerIndex + 1} joined using {device.displayName}");
    }

    private void UpdatePlayerStatus(int playerIndex)
    {
        if (playerIndex >= playerStatusTexts.Length ||
            playerStatusTexts[playerIndex] == null)
            return;

        playerStatusTexts[playerIndex].text =
            playerReadyStates[playerIndex]
                ? "READY!"
                : "Joined - Not Ready";
    }
    
    private sealed class ButtonPressObserver : IObserver<InputControl>
    {
        private readonly Action<InputControl> onPress;

        public ButtonPressObserver(Action<InputControl> onPress)
        {
            this.onPress = onPress;
        }

        public void OnNext(InputControl control)
        {
            onPress(control);
        }

        public void OnError(Exception error)
        {
            Debug.LogException(error);
        }

        public void OnCompleted() { }
    }
}