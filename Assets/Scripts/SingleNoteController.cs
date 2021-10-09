using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleNoteController : NoteControllerBase
{
    [SerializeField] AudioClip clipHit; // ���ʉ�

    // Update is called once per frame
    void Update() {
        SetTransform();
        CheckMiss();
    }


    // ���W�ݒ�
    private void SetTransform() {
        Vector3 position = new Vector3();
        position.x = (float)noteProperty.lane - 3.5f;
        position.z = (noteProperty.beatBegin - PlayerController.CurrentBeat) * PlayerController.ScrollSpeed;
        position.y = 0.0f;

        transform.localPosition = position;
    }

    // ���������o
    private void CheckMiss() {
        // Bad�̔��蕝��ʉ�
        if (noteProperty.secBegin - PlayerController.CurrentSec < -JudgementManager.JudgementWidth[JudgementType.Bad]) {
            // �~�X����
            EvaluationManager.OnMiss();

            // �������m�[�c�ꗗ����폜
            PlayerController.ExistingNoteControllers.Remove(
                GetComponent<NoteControllerBase>()
                );
            // GameObject���̂��폜
            Destroy(gameObject);
        }
    }

    public override void OnKeyDown(JudgementType judgementType) {
        // �f�o�b�O�p�ɃR���\�[���ɔ�����o��
        Debug.Log(judgementType);

        if (judgementType != JudgementType.Poor) {
            // �q�b�g����
            EvaluationManager.OnHit(judgementType);

            // ���ʉ��Đ�
            AudioSource.PlayClipAtPoint(clipHit, transform.position);
            // �������m�[�c�ꗗ����폜
            PlayerController.ExistingNoteControllers.Remove(
                GetComponent<NoteControllerBase>());
            Destroy(gameObject);
        }

    }
}
