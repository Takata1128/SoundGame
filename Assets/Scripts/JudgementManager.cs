using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class JudgementManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [SerializeField] private JudgementUIManager judgementUIManager;

    public static Dictionary<JudgementType, float> JudgementWidth = new Dictionary<JudgementType, float> {
        { JudgementType.Perfect, 0.05f }, // perfect�̔��蕝
        { JudgementType.Great, 0.10f }, // great�̔��蕝
        { JudgementType.Good, 0.20f }, // good�̔��蕝
        { JudgementType.Bad, 0.30f }, // bad�̔��蕝
    };

    private static KeyCode[] InputKeys = new KeyCode[]
        {
            KeyCode.D,
            KeyCode.F,
            KeyCode.G,
            KeyCode.H,
            KeyCode.J,
            KeyCode.K,
            KeyCode.L,
            KeyCode.Space,
            KeyCode.RightShift
        };

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckBG();
        CheckMiss();
        CheckInput();
    }

    private void CheckBG()
    {
        while (PlayerController.ExistingBGSoundControllers.Any() && PlayerController.ExistingBGSoundControllers.Back.Note.SecBegin < PlayerController.CurrentSec)
        {
            BgmController bgmController = PlayerController.ExistingBGSoundControllers.Back;
            playerController.SoundManager.PlayKeySound(bgmController.Note.KeySound);
            Destroy(bgmController.gameObject);
            PlayerController.ExistingBGSoundControllers.Pop();
        }
    }

    private void CheckMiss()
    {
        for (int lane = 1; lane < 9; lane++)
        {
            if (PlayerController.ExistingNoteControllers[lane].Count == 0) continue;
            var noteController = PlayerController.ExistingNoteControllers[lane].Back;
            if (noteController.CheckMiss())
            {
                Destroy(noteController.gameObject);
                PlayerController.ExistingNoteControllers[lane].Pop();
                judgementUIManager.ShowJudge(JudgementType.Poor);
            }

        }
    }

    private void CheckInput()
    {
        for (int lane = 1; lane < 9; lane++)
        {
            var inputKey = InputKeys[lane - 1];

            if (Input.GetKeyDown(inputKey))
            {
                // キービーム(TODO リッチにする)
                if (lane <= 7)
                    GameObject.Find("TapPosition" + (lane - 1).ToString()).GetComponent<Renderer>().material.color = Color.blue;

                // 最近のノーツを処理
                var nearest = GetNearestNoteControllerBaseInLane(lane);
                if (!nearest) continue;
                var noteSec = nearest.Note.SecBegin;
                var differenceSec = Mathf.Abs(noteSec - PlayerController.CurrentSec);
                var judge = GetJudgementType(differenceSec);
                nearest.OnKeyDown(judge);
                playerController.SoundManager.PlayKeySound(nearest.Note.KeySound);


                if (judge != JudgementType.Poor)
                {
                    Destroy(nearest.gameObject);
                    PlayerController.ExistingNoteControllers[lane].Pop();
                }
                judgementUIManager.ShowJudge(judge);
            }
            else if (Input.GetKeyUp(inputKey))
            {
                // キービーム(TODO リッチにする)
                if (lane <= 7)
                    GameObject.Find("TapPosition" + (lane - 1).ToString()).GetComponent<Renderer>().material.color = Color.red;

                var processed = GetProcessedNoteControllerBaseInLane(lane);
                if (!processed) continue;

                var noteSec = processed.Note.SecEnd;
                var differenceSec = Mathf.Abs(noteSec - PlayerController.CurrentSec);

                var judge = GetJudgementType(differenceSec);

                processed.OnKeyUp(judge);
                playerController.SoundManager.PlayKeySound(processed.Note.KeySound);
                if (judge != JudgementType.Poor)
                {
                    Destroy(processed.gameObject);
                    PlayerController.ExistingNoteControllers[lane].Pop();
                }
                judgementUIManager.ShowJudge(judge);

            }
        }
    }

    private JudgementType GetJudgementType(float differenceSec)
    {
        // Perfect
        if (differenceSec <= JudgementWidth[JudgementType.Perfect])
        {
            return JudgementType.Perfect;
        }
        // Great
        else if (differenceSec <= JudgementWidth[JudgementType.Great])
        {
            return JudgementType.Great;
        }
        // Good
        else if (differenceSec <= JudgementWidth[JudgementType.Good])
        {
            return JudgementType.Good;
        }
        // Bad
        else if (differenceSec <= JudgementWidth[JudgementType.Bad])
        {
            return JudgementType.Bad;
        }
        // Other
        else
        {
            return JudgementType.Poor;
        }


    }

    private NoteControllerBase GetNearestNoteControllerBaseInLane(int lane)
    {
        // �w�肵�����[�����̃m�[�c
        var noteControllers = PlayerController.ExistingNoteControllers[lane];

        // �m�[�c������
        if (noteControllers.Any())
        {
            // beat�̍��̐�Βl���ł����������̂�Ԃ�
            return noteControllers.Back;
        }
        return null;
    }

    private NoteControllerBase GetProcessedNoteControllerBaseInLane(int lane)
    {
        // �w�肵�����[�����̃m�[�c
        var noteControllers = PlayerController.ExistingNoteControllers[lane].Where(x => x.IsProcessed);

        // �m�[�c������
        if (noteControllers.Any())
        {
            // beat�̍��̐�Βl���ł����������̂�Ԃ�
            return noteControllers.OrderBy(x => Mathf.Abs(x.Note.BeatBegin - PlayerController.CurrentBeat)).First();
        }
        return null;
    }
}

public enum JudgementType
{
    Perfect,
    Great,
    Good,
    Bad,
    Poor
}
