using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public AudioManager AudioManager;

    [SerializeField] private GameObject start;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject back;

    [SerializeField] private GameObject sfxSlider;
    [SerializeField] private GameObject masterSlider;
    [SerializeField] private GameObject musicSlider;

    public void Awake()
    {
        // Vind de audiomanager in de scene
        AudioManager = FindObjectOfType<AudioManager>();

        // Voeg geluiden toe aan buttons
        start.GetComponent<Button>().onClick.AddListener(delegate { AudioManager.PlaySound("MenuButton"); });
        options.GetComponent<Button>().onClick.AddListener(delegate { AudioManager.PlaySound("MenuButton"); });
        back.GetComponent<Button>().onClick.AddListener(delegate { AudioManager.PlaySound("MenuButton"); });

        // Maak opties sliders functioneel
        sfxSlider.GetComponent<Slider>().onValueChanged.AddListener(delegate { AudioManager.SetSoundsVolume(sfxSlider.GetComponent<Slider>().value); });
        masterSlider.GetComponent<Slider>().onValueChanged.AddListener(delegate { AudioManager.SetMasterVolume(masterSlider.GetComponent<Slider>().value); });
        musicSlider.GetComponent<Slider>().onValueChanged.AddListener(delegate { AudioManager.SetMusicVolume(musicSlider.GetComponent<Slider>().value); });

    }
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
