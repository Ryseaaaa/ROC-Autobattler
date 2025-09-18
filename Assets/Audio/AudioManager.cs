using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{

    public enum MixerGroups
    {
        Sound,
        Music,
        Master
    }


    [SerializeField]
    private AudioMixer mixer;

    [SerializeField] 
    public Sound[] Sounds;

    private Sound curMusic = null;

    public void Awake()
    {
        if(FindObjectsOfType<AudioManager>().Length >= 2)
        {
            Destroy(gameObject);
        }
        foreach(Sound s  in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.loop;

            s.Source.outputAudioMixerGroup = getMixerGroup(s.MixerGroup);

        }
        PlayMusic("MenuMusic");
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
    }

    private readonly float cutoffTime = 0.8f;
    public void Update()
    {
        mixer.GetFloat("MusicLP", out float curValue);
        mixer.SetFloat("MusicLP", Mathf.Lerp(curValue,targetCutoff,Time.deltaTime/cutoffTime));
    }


    private Sound getSound(string name)
    {
        Sound sound = Array.Find(Sounds, sound => sound.Name == name);
        if(sound == null)
        {
            Debug.Log("Could not find sound: \""+name+"\"");
        }
        return sound;
    }
    public void PlaySound(string name)
    {
        getSound(name).Source.Play();
    }

    

    public void PlayMusic(string name)
    {
        if (!(curMusic == null))
        {
            curMusic.Source.Stop();
        }
        curMusic = getSound(name);
        curMusic.Source.Play();

    }


    private AudioMixerGroup getMixerGroup(MixerGroups group)
    {
        return mixer.FindMatchingGroups(group.ToString())[0];
    }
    private int targetCutoff = 22000;
    public void MusicFilter(bool toggle)
    {
        targetCutoff = toggle ? 22000 : 500;
    }

    public void SetSoundsVolume(System.Single volume)
    {
        mixer.SetFloat("SoundsVol", volume);
    }
    public void SetMusicVolume(System.Single volume)
    {
        mixer.SetFloat("MusicVol", volume);
    }
    public void SetMasterVolume(System.Single volume)
    {
        mixer.SetFloat("MasterVol", volume);
    }
}
