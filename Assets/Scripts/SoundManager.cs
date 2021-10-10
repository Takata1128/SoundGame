using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


public class SoundManager : MonoBehaviour
{
    public bool IsPrepared { get; set; } = false;

    public Dictionary<string, string> SoundData { get; set; }

    public BMSMultiChannelAudioSource Src;

    public Dictionary<int, string> Pathes { get; set; }
    private Dictionary<int, AudioClip> Clips { get; set; }

    private static string[] SoundExtensions;
    private void Awake()
    {
        Pathes = new Dictionary<int, string>();
        Clips = new Dictionary<int, AudioClip>();
    }

    public void AddAudioClips()
    {
        StartCoroutine(CAddAudioClips());
    }

    private IEnumerator CAddAudioClips()
    {
        foreach (KeyValuePair<int, string> p in Pathes)
        {
            string filePath = p.Value;
            if (filePath.Contains("#"))
            {
                filePath = filePath.Replace("#", "%23");
            }
            string url = PlayerController.BmsHeader.ParentPath + System.IO.Path.DirectorySeparatorChar + filePath;
            AudioType type = GetAudioType(url);
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + url, type);

            yield return www.SendWebRequest();

            if (www.downloadHandler.data.Length != 0)
            {
                AudioClip c = DownloadHandlerAudioClip.GetContent(www);
                c.LoadAudioData();
                Clips.Add(p.Key, c);
            }
            else
            {
                Debug.LogWarning($"Failed to read sound data : {www.url}");
            }
        }
        IsPrepared = true;
    }

    public void PlayKeySound(int key, float volume = 1.0f)
    {
        if (Clips.ContainsKey(key))
        {
            Src.PlayOneShot(Clips[key], volume);
        }
    }

    // ファイル名から音源のフォーマットを取得する
    private AudioType GetAudioType(string filePath)
    {
        // 拡張子を取得
        string ext = System.IO.Path.GetExtension(filePath).ToLower();
        switch (ext)
        {
            case ".ogg":
                return AudioType.OGGVORBIS;
            case ".mp3":
                return AudioType.MPEG;
            case ".wav":
                return AudioType.WAV;
            default:
                return AudioType.UNKNOWN;
        }
    }
}