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

    [SerializeField]
    private readonly float cutoffTime = 0.8f;

    private Sound curMusic = null;

    private int targetCutoff = 22000;

    public void Awake()
    {
        //if theres another audiomanager, destroy self
        if(FindObjectsOfType<AudioManager>().Length >= 2)
        {
            Destroy(gameObject);
        }
        //add sounds to audiomanager object as audiosource
        foreach(Sound s in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.loop;

            s.Source.outputAudioMixerGroup = getMixerGroup(s.MixerGroup);

        }
        //play menu music on awake
        PlayMusic("MenuMusic");
        DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {
        //set low pass filter cutoff
        mixer.GetFloat("MusicLP", out float curValue);
        mixer.SetFloat("MusicLP", Mathf.Lerp(curValue,targetCutoff,Time.deltaTime/cutoffTime));
    }

    //gets the sound in sound array by name
    private Sound getSound(string name)
    {
        Sound sound = Array.Find(Sounds, sound => sound.Name == name);
        if(sound == null)
        {
            Debug.Log("Could not find sound: \""+name+"\"");
        }
        return sound;
    }

    //plays sound
    public void PlaySound(string name)
    {
        getSound(name).Source.Play();
    }

    
    //plays music and stops current music if it exists
    public void PlayMusic(string name)
    {
        if (!(curMusic == null))
        {
            curMusic.Source.Stop();
        }
        curMusic = getSound(name);
        curMusic.Source.Play();

    }

    //returns the mixergroup
    private AudioMixerGroup getMixerGroup(MixerGroups group)
    {
        return mixer.FindMatchingGroups(group.ToString())[0];
    }

    //sets the music filter on or off
    public void MusicFilter(bool toggle)
    {
        targetCutoff = toggle ? 22000 : 500;
    }

    // sets the volume of the sound mixer channel
    public void SetSoundsVolume(System.Single volume)
    {
        mixer.SetFloat("SoundsVol", volume);
    }
    // sets the volume of the music mixer channel
    public void SetMusicVolume(System.Single volume)
    {
        mixer.SetFloat("MusicVol", volume);
    }
    // sets the volume of the master mixer channel
    public void SetMasterVolume(System.Single volume)
    {
        mixer.SetFloat("MasterVol", volume);
    }
}
