using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.EventSystems;

public class PlayerLobbyManager : MonoBehaviour
{
    [SerializeField] private TMP_Text[] playerStatusTexts;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private Button startGameButton;
    [SerializeField] private GameObject defaultSelectedButton;

    private readonly List<InputDevice> joinedDevices = new();
    private readonly List<bool> playerReadyStates = new();
    private IDisposable buttonSubscription;
    
    private void Update()
    {
        if (lobbyPanel == null || !lobbyPanel.activeInHierarchy)
            return;

        if (EventSystem.current == null || Gamepad.all.Count == 0)
            return;

        // Only restore UI selection when a controller tries to navigate.
        foreach (Gamepad gamepad in Gamepad.all)
        {
            bool navigating =
                gamepad.dpad.up.wasPressedThisFrame ||
                gamepad.dpad.down.wasPressedThisFrame ||
                gamepad.dpad.left.wasPressedThisFrame ||
                gamepad.dpad.right.wasPressedThisFrame ||
                gamepad.leftStick.ReadValue().sqrMagnitude > 0.5f;

            if (!navigating)
                continue;

            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
            }

            break;
        }
    }
    
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
        
        UpdateStartButton();
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
                (device is Keyboard && control == Keyboard.current.rKey) ||
                (device is Gamepad && control == ((Gamepad)device).buttonNorth);

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
        
        UpdateStartButton();
    }
    
    public bool SavePlayersForGame()
    {
        if (joinedDevices.Count == 0)
            return false;

        foreach (bool isReady in playerReadyStates)
        {
            if (!isReady)
                return false;
        }

        MenuPlayerSelection.SetPlayers(joinedDevices);
        return true;
    }
    
    private void UpdateStartButton()
    {
        bool canStart = joinedDevices.Count > 0;

        foreach (bool isReady in playerReadyStates)
        {
            if (!isReady)
            {
                canStart = false;
                break;
            }
        }

        if (startGameButton != null)
            startGameButton.interactable = canStart;
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