using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    private AudioManager audioManager;

    //find audio manager
    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.Log("Couldnt find audio manager");
        }
    }

    //restart game button
    public void RestartGame()
    {
        if (audioManager != null)
        {
            audioManager.MusicFilter(true);
        }
        SceneManager.LoadScene("Game");
    }

    //go to main menu button
    public void MainMenu()
    {
        if (audioManager != null)
        {
            audioManager.MusicFilter(true);
            audioManager.PlayMusic("MenuMusic");
        }
        SceneManager.LoadScene("MainMenu");
    }

    //exit game button
    public void ExitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
