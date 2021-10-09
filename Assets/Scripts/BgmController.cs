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

    // �w�肳�ꂽ�p�X�̉�����ǂݍ���
    private IEnumerator LoadAudioFile(string filePath) {
        // �t�@�C�������݂��Ȃ�
        if (!File.Exists(filePath)) { yield break; }
        // �����̃t�H�[�}�b�g���
        var audioType = GetAudioType(filePath);

        // UnityWebRequest�ŊO�����\�[�X�ǂݍ���
        using (var request = UnityWebRequestMultimedia.GetAudioClip(
            "file:///" + filePath, audioType
            )) {
            yield return request.SendWebRequest();
            // �G���[������
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError(request.error);
                yield break;
            }
            // �I�[�f�B�I�N���b�v�ǂݍ���
            var audioClip = DownloadHandlerAudioClip.GetContent(request);
            // audioSource��clip�ɐݒ�
            audioSource.clip = audioClip;
        }
    }

    private AudioType GetAudioType(string filePath) {
        // �g���q
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
