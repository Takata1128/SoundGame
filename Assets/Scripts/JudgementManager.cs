using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JudgementManager : MonoBehaviour
{
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
        };

    // Update is called once per frame
    void Update() {
        // �e���[���ɑ΂��ď���
        for (int lane = 0; lane < InputKeys.Length; lane++) {
            // ���[���ɑΉ�����L�[
            var inputKey = InputKeys[lane];

            // �L�[������������
            if (Input.GetKeyDown(inputKey)) {
                // �ŋߖT�m�[�c
                var nearest = GetNearestNoteControllerBaseInLane(lane);
                if (!nearest) continue;

                // �������ׂ��^�C�~���O
                var noteSec = nearest.noteProperty.secBegin;
                // ���ۂɉ������^�C�~���O�Ƃ̍���
                var differenceSec = Mathf.Abs(noteSec - PlayerController.CurrentSec);

                // ���菈��
                nearest.OnKeyDown(GetJudgementType(differenceSec));
            }
            // �L�[�𗣂�����
            else if (Input.GetKeyUp(inputKey)) {
                // �������̃m�[�c
                var processed = GetProcessedNoteControllerBaseInLane(lane);
                if (!processed) continue;

                // �������ׂ��^�C�~���O
                var noteSec = processed.noteProperty.secEnd;
                // ���ۂɗ������^�C�~���O�Ƃ̍���
                var differenceSec = Mathf.Abs(noteSec - PlayerController.CurrentSec);

                // ���菈��
                processed.OnKeyUp(GetJudgementType(differenceSec));
            }
        }
    }

    private JudgementType GetJudgementType(float differenceSec) {
        // Perfect
        if (differenceSec <= JudgementWidth[JudgementType.Perfect]) {
            return JudgementType.Perfect;
        }
        // Great
        else if (differenceSec <= JudgementWidth[JudgementType.Great]) {
            return JudgementType.Great;
        }
        // Good
        else if (differenceSec <= JudgementWidth[JudgementType.Good]) {
            return JudgementType.Good;
        }
        // Bad
        else if (differenceSec <= JudgementWidth[JudgementType.Bad]) {
            return JudgementType.Bad;
        }
        // Other
        else {
            return JudgementType.Poor;
        }


    }

    private NoteControllerBase GetNearestNoteControllerBaseInLane(int lane) {
        // �w�肵�����[�����̃m�[�c
        var noteControllers = PlayerController.ExistingNoteControllers.Where(x => x.noteProperty.lane == lane);

        // �m�[�c������
        if (noteControllers.Any()) {
            // beat�̍��̐�Βl���ł����������̂�Ԃ�
            return noteControllers.OrderBy(x => Mathf.Abs(x.noteProperty.beatBegin - PlayerController.CurrentBeat)).First();
        }
        return null;
    }

    private NoteControllerBase GetProcessedNoteControllerBaseInLane(int lane) {
        // �w�肵�����[�����̃m�[�c
        var noteControllers = PlayerController.ExistingNoteControllers.Where(x => x.noteProperty.lane == lane && x.isProcessed);

        // �m�[�c������
        if (noteControllers.Any()) {
            // beat�̍��̐�Βl���ł����������̂�Ԃ�
            return noteControllers.OrderBy(x => Mathf.Abs(x.noteProperty.beatBegin - PlayerController.CurrentBeat)).First();
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