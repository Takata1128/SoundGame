using System.Collections;
using UnityEngine;
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

    public static BmsData BmsData;
    [SerializeField] public SoundManager SoundManager;

    private float startOffset = 1.0f; // ���ʂ̃I�t�Z�b�g�i�b�j
    private float startSec = 0f; // ���ʍĐ��J�n�b�� 
    private bool isPlaying = false; // ���ʒ�~�����ۂ�

    private IEnumerator PreLoad()
    {
        SoundManager.Pathes = BmsData.BmsHeader.SoundPathes;
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

        foreach (var bpm in BmsData.BmsScore.Bpms)
        {
            Debug.Log(bpm.BeatBegin + ": " + bpm.Bpm);
        }

        // Bgm�I�u�W�F�N�g�̐���
        MakeObject(BmsData.BmsScore);
        StartCoroutine(PreLoad());
    }



    // Update is called once per frame
    void Update()
    {
        if (!isPlaying && Input.GetKeyDown(KeyCode.Return))
        {
            isPlaying = true;
        }
        if (!isPlaying)
        {
            startSec = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.sceneLoaded += GameSceneLoaded;
            SceneManager.LoadScene("ResultScene");
        }

        CurrentSec = Time.time - startOffset - startSec;
        CurrentBeat = Util.ToBeat(CurrentSec, BmsData.BmsScore.Bpms);
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        var resultController = GameObject.Find("ResultController").GetComponent<ResultController>();
        resultController.JudgementCounts = EvaluationManager.judgementCounts;
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    private void MakeObject(BMSScore BmsScore)
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
                objNote.GetComponent<NoteControllerBase>().Note = note;
                objNote.GetComponent<NoteControllerBase>().Lane = lane;
            }
        }

        foreach (BGNote note in BmsScore.BGSounds)
        {
            GameObject bgObj = Instantiate(prefabBgmObject);
            ExistingBGSoundControllers.Add(bgObj.GetComponent<BgmController>());
            bgObj.GetComponent<BgmController>().Note = note;
        }
    }
}
