using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void RestartGame()
    {
        FindObjectOfType<AudioManager>().MusicFilter(true);
        SceneManager.LoadScene("Game");
    }
    public void MainMenu()
    {
        FindObjectOfType<AudioManager>().MusicFilter(true);
        FindObjectOfType<AudioManager>().PlayMusic("MenuMusic");
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
