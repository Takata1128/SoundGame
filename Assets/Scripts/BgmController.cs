using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;

public class BgmController : MonoBehaviour
{
    public BgmProperty bgmProperty; // BgmProperty
    [SerializeField] public AudioSource audioSource;

    public void LoadAudio(string filePath) {
        StartCoroutine(LoadAudioFile(filePath));
    }

    // 指定されたパスの音源を読み込む
    private IEnumerator LoadAudioFile(string filePath) {
        // ファイルが存在しない
        if (!File.Exists(filePath)) { yield break; }
        // 音源のフォーマット種別
        var audioType = GetAudioType(filePath);

        // UnityWebRequestで外部リソース読み込み
        using (var request = UnityWebRequestMultimedia.GetAudioClip(
            "file:///" + filePath, audioType
            )) {
            yield return request.SendWebRequest();
            // エラーが発生
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError(request.error);
                yield break;
            }
            // オーディオクリップ読み込み
            var audioClip = DownloadHandlerAudioClip.GetContent(request);
            // audioSourceのclipに設定
            audioSource.clip = audioClip;
        }
    }

    private AudioType GetAudioType(string filePath) {
        // 拡張子
        string ext = Path.GetExtension(filePath).ToLower();
        switch (ext) {
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
