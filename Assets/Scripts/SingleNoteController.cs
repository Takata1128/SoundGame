using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleNoteController : NoteControllerBase
{

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        SetTransform();
    }


    // ���W�ݒ�
    private void SetTransform()
    {
        Vector3 position = new Vector3();
        position.x = (float)Lane - 4.5f;
        position.z = (Note.BeatBegin - PlayerController.CurrentBeat) * PlayerController.ScrollSpeed;
        position.y = 0.0f;
        transform.localPosition = position;
    }

    // ���������o
    public override bool CheckMiss()
    {
        // Bad�̔��蕝��ʉ�
        if (Note.SecBegin - PlayerController.CurrentSec < -JudgementManager.JudgementWidth[JudgementType.Bad])
        {
            // �~�X����
            EvaluationManager.OnMiss();
            // �������m�[�c�ꗗ����폜
            return true;
        }
        return false;
    }

    public override void OnKeyDown(JudgementType judgementType)
    {
        // �f�o�b�O�p�ɃR���\�[���ɔ�����o��
        Debug.Log(judgementType);
        if (judgementType != JudgementType.Poor)
        {
            // �q�b�g����
            EvaluationManager.OnHit(judgementType);
            // ���ʉ��Đ�
            Vector3 pos = gameObject.transform.position;
            var burstEffect = Instantiate(prefabBurst, pos, Quaternion.identity);
            burstEffect.transform.localScale *= burstSize;
            Destroy(burstEffect, deleteTime);
        }

    }
}
