using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "3dPersonTest";
    [SerializeField] private GameObject playerLobbyPanel;
    [SerializeField] private PlayerLobbyManager playerLobbyManager;
    [SerializeField] private GameObject playButton;

    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerLobbyPanel.SetActive(false);

        if (EventSystem.current != null && playButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(playButton);
        }
    }
    
    public void PlayGame()
    {
        playerLobbyPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        GameObject backButton = playerLobbyPanel.transform
            .Find("BackButton").gameObject;

        EventSystem.current.SetSelectedGameObject(backButton);
    }

    public void BackToMenu()
    {
        // Close the lobby.
        playerLobbyPanel.SetActive(false);

        // Reset joined players and ready states.
        playerLobbyManager.ResetLobby();

        // Clear the previous UI selection.
        EventSystem.current.SetSelectedGameObject(null);

        // Select Play so keyboard/controller navigation works.
        EventSystem.current.SetSelectedGameObject(playButton);
    }

    
    public void StartGameplay()
    {
        if (playerLobbyManager == null ||
            !playerLobbyManager.SavePlayersForGame())
        {
            return;
        }

        SceneManager.LoadScene(gameplaySceneName);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}