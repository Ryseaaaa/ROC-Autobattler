using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame(){
        FindObjectOfType<AudioManager>().MusicFilter(true);
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
