using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    private AudioManager audioManager;
    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.Log("Couldnt find audio manager");
        }
    }
    public void RestartGame()
    {
        if (audioManager != null)
        {
            audioManager.MusicFilter(true);
        }
        SceneManager.LoadScene("Game");
    }
    public void MainMenu()
    {
        if (audioManager != null)
        {
            audioManager.MusicFilter(true);
            audioManager.PlayMusic("MenuMusic");
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
