using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject prefabSingleNote; // ��������prefab
    [SerializeField] private GameObject prefabLongNote;
    [SerializeField] private GameObject prefabBgmObject;

    public static float ScrollSpeed = 1.0f;
    public static float CurrentSec = 0f;
    public static float CurrentBeat = 0f;

    // ���菈���ŏ����Ă��Ȃ��m�[�c�ꗗ
    public static List<NoteControllerBase> ExistingNoteControllers;

    // Bgm�����ŏ����Ă��Ȃ��I�u�W�F�N�g�ꗗ
    public static List<BgmController> ExistingBgmControllers;

    public static Beatmap beatmap; // ���ʃf�[�^�Ǘ�
    private float startOffset = 1.0f; // ���ʂ̃I�t�Z�b�g�i�b�j
    private float startSec = 0f; // ���ʍĐ��J�n�b�� 
    private bool isPlaying = false; // ���ʒ�~�����ۂ�


    // Start is called before the first frame update
    void Awake()
    {
        CurrentSec = 0f;
        CurrentBeat = 0f;

        // �������m�[�c�ꗗ��������
        ExistingNoteControllers = new List<NoteControllerBase>();
        ExistingBgmControllers = new List<BgmController>();

        // �f�o�b�O�p�Ƀe���|�ω����R���\�[���ɏo��
        //foreach (var tempoChange in beatmap.tempoChanges) {
        //    Debug.Log(tempoChange.beat + ": " + tempoChange.tempo);
        //}

        // Bgm�I�u�W�F�N�g�̐���
        foreach (var bgmProperty in beatmap.bgmProperties)
        {
            GameObject objBgm = Instantiate(prefabBgmObject);
            BgmController controller = objBgm.GetComponent<BgmController>();
            ExistingBgmControllers.Add(controller);
            controller.bgmProperty = bgmProperty;
            controller.LoadAudio(beatmap.audioFilePaths[bgmProperty.bgmIndex]);
        }

        // �m�[�c�̐���
        foreach (var noteProperty in beatmap.noteProperties)
        {
            GameObject objNote = null;
            switch (noteProperty.noteType)
            {
                case NoteType.Single:
                    objNote = Instantiate(prefabSingleNote);
                    break;
                case NoteType.Long:
                    objNote = Instantiate(prefabLongNote);
                    break;
            }
            // �������m�[�c�ꗗ�ɒǉ�
            ExistingNoteControllers.Add(objNote.GetComponent<NoteControllerBase>());
            objNote.GetComponent<NoteControllerBase>().noteProperty = noteProperty;
        }


    }

    // Update is called once per frame
    void Update()
    {
        // ���ʒ�~���ɃX�y�[�X���������Ƃ�
        if (!isPlaying && Input.GetKeyDown(KeyCode.Space))
        {
            // ���ʍĐ�
            isPlaying = true;
            foreach (var bgmController in ExistingBgmControllers)
            {
                bgmController.audioSource.PlayScheduled(
                    AudioSettings.dspTime + startOffset + bgmController.bgmProperty.secBegin
                );
            }
        }
        // ���ʒ�~��
        if (!isPlaying)
        {
            // startSec�X�V
            startSec = Time.time;
        }
        // �Q�[���I��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("SelectScene");
        }

        // �b�����X�V
        CurrentSec = Time.time - startOffset - startSec;

        // �����X�V
        CurrentBeat = Beatmap.ToBeat(CurrentSec, beatmap.tempoChanges);
        // CurrentBeat = Util.ToBeat(CurrentSec);
    }
}
