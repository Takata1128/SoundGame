using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SoundManager : MonoBehaviour
{
    public bool IsPrepared { get; set; } = false;
    public GameObject KeySoundObject;
    public BMSMultiChannelAudioSource Src;

    public Dictionary<int, string> Pathes { get; set; }
    public Dictionary<int, AudioClip> Clips { get; set; }

    private static string[] SoundExtensions;
    private void Awake()
    {
        Pathes = new Dictionary<int, string>();
        Clips = new Dictionary<int, AudioClip>();
        SoundExtensions = new string[] { ".ogg", ".wav", ".mp3" };
    }

    public void AddAudioClips()
    {
        // StartCoroutine(CAddAudioClips());
    }

    // private IEnumerator CAddAudioClips()
    // {
    //     foreach (KeyValuePair<int, string> p in Pathes)
    //     {

    //     }
    // }

    public void PlayKeySound(int key, float volume = 1.0f)
    {
        if (Clips.ContainsKey(key))
        {
            Src.PlayOneShot(Clips[key], volume);
        }
    }
}