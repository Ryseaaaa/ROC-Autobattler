using UnityEngine.Audio;
using UnityEngine;
using System.ComponentModel;
using System;

//class to store sound clips and metadata, and make them usable in the unity editor
[System.Serializable]
public class Sound 
{
    

    public string Name;
    public AudioClip Clip;

    [Range(0f, 1f)]
    public float Volume = 1;

    [Range(0f, 2f)]
    public float Pitch = 1;

    public AudioManager.MixerGroups MixerGroup;

    public bool loop;

    [HideInInspector]
    public AudioSource Source;
}
