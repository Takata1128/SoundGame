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
    public static MyList<NoteControllerBase>[] ExistingNoteControllers;

    public static MyList<BgmController> ExistingBGSoundControllers;

    public static List<BmsLoader> BmsLoaders;
    public static BMSHeader BmsHeader;
    public static BMSScore BmsScore;
    [SerializeField] public SoundManager SoundManager;

    private float startOffset = 1.0f; // ���ʂ̃I�t�Z�b�g�i�b�j
    private float startSec = 0f; // ���ʍĐ��J�n�b�� 
    private bool isPlaying = false; // ���ʒ�~�����ۂ�

    private IEnumerator PreLoad()
    {
        SoundManager.Pathes = BmsHeader.SoundPathes;
        SoundManager.AddAudioClips();
        yield return new WaitUntil(() => SoundManager.IsPrepared);
        Debug.Log("Game starts!");
    }


    // Start is called before the first frame update
    void Awake()
    {
        CurrentSec = 0f;
        CurrentBeat = 0f;

        // �������m�[�c�ꗗ��������
        ExistingNoteControllers = new MyList<NoteControllerBase>[10];
        ExistingBGSoundControllers = new MyList<BgmController>();
        for (int i = 0; i < 10; i++)
        {
            ExistingNoteControllers[i] = new MyList<NoteControllerBase>();
        }

        foreach (var bpm in BmsScore.Bpms)
        {
            Debug.Log(bpm.BeatBegin + ": " + bpm.Bpm);
        }

        // Bgm�I�u�W�F�N�g�̐���
        MakeObject(BmsScore);
        StartCoroutine(PreLoad());
    }



    // Update is called once per frame
    void Update()
    {
        if (!isPlaying && Input.GetKeyDown(KeyCode.Space))
        {
            isPlaying = true;
        }
        if (!isPlaying)
        {
            startSec = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SelectorController.BmsLoaders = BmsLoaders;
            SceneManager.LoadScene("SelectScene");
        }

        CurrentSec = Time.time - startOffset - startSec;
        CurrentBeat = Util.ToBeat(CurrentSec, BmsScore.Bpms);
    }

    void MakeObject(BMSScore BmsScore)
    {
        for (int lane = 0; lane < BmsScore.Lanes.Length; lane++)
        {
            foreach (Note note in BmsScore.Lanes[lane].NoteList)
            {
                GameObject objNote = null;
                if (note is LongNote)
                {
                    objNote = Instantiate(prefabLongNote);
                }
                else
                { // Note
                    objNote = Instantiate(prefabSingleNote);
                }
                ExistingNoteControllers[lane].Add(objNote.GetComponent<NoteControllerBase>());
                objNote.GetComponent<NoteControllerBase>().note = note;
                objNote.GetComponent<NoteControllerBase>().lane = lane;
            }
        }

        foreach (BGNote note in BmsScore.BGSounds)
        {
            GameObject bgObj = Instantiate(prefabBgmObject);
            ExistingBGSoundControllers.Add(bgObj.GetComponent<BgmController>());
            bgObj.GetComponent<BgmController>().note = note;
        }
    }
}
