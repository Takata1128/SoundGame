using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteProperty : NoteControllerBase
{
    [SerializeField] GameObject objBegin; // �n�_������GameObject
    [SerializeField] GameObject objTrail; // �O�Օ�����GameObject
    [SerializeField] GameObject objEnd; // �I�_������GameObject

    [SerializeField] Color32 processedColorEdges; // �������̎n�_�I�_�̐F
    [SerializeField] Color32 processedColorTrail; // �������̋O�Ղ̐F

    [SerializeField] AudioClip clipHit;

    // Update is called once per frame
    void Update() {
        SetTransform();
        CheckMiss();
    }

    private void SetTransform() {

        // �n�_�̍��W
        Vector3 positionBegin = new Vector3();
        positionBegin.x = (float)noteProperty.lane - 3.5f;
        float zBegin = (noteProperty.beatBegin - PlayerController.CurrentBeat) * PlayerController.ScrollSpeed;
        positionBegin.y = 0.0f;
        positionBegin.z = isProcessed ? objBegin.transform.position.z : zBegin;
        objBegin.transform.localPosition = positionBegin;

        // �I�_�̍��W
        Vector3 positionEnd = new Vector3();
        positionEnd.x = (float)noteProperty.lane - 3.5f;
        float zEnd = (noteProperty.beatEnd - PlayerController.CurrentBeat) * PlayerController.ScrollSpeed;
        positionEnd.y = 0.0f;
        positionEnd.z = zEnd;
        objEnd.transform.localPosition = positionEnd;


        // �O�Օ����̍��W�i�n�_�ƏI�_�̒��S�j
        Vector3 positionTrail = (positionBegin + positionEnd) / 2.0f;
        objTrail.transform.localPosition = positionTrail;


        // �O�Օ����̊g�嗦
        Vector3 scale = objTrail.transform.localScale;
        scale.z = Vector3.Distance(objBegin.transform.position, objEnd.transform.position);
        objTrail.transform.localScale = scale;

    }

    private void CheckMiss() {
        // �������łȂ� && �n�_��BAD���蕝�𒴂���
        if (isProcessed && noteProperty.secBegin - PlayerController.CurrentSec < -JudgementManager.JudgementWidth[JudgementType.Bad]) {
            // �~�X����
            EvaluationManager.OnMiss(); // �n�_
            EvaluationManager.OnMiss(); // �I�_

            // �폜
            PlayerController.ExistingNoteControllers.Remove(
                GetComponent<NoteControllerBase>());
            Destroy(gameObject);
        }

        // ������ && �I�_��BAD���蕝�𒴂���
        if (isProcessed && noteProperty.secEnd - PlayerController.CurrentSec < -JudgementManager.JudgementWidth[JudgementType.Bad]) {
            // �~�X����
            EvaluationManager.OnMiss();
            // �������t���O����
            isProcessed = false;

            // �폜
            PlayerController.ExistingNoteControllers.Remove(
                GetComponent<NoteControllerBase>());
            Destroy(gameObject);
        }
    }

    public override void OnKeyDown(JudgementType judgementType) {
        // �R���\�[���ɔ���\��
        Debug.Log(judgementType);

        // ���肪Poor�łȂ�
        if (judgementType != JudgementType.Poor) {
            // �q�b�g����
            EvaluationManager.OnHit(judgementType);
            // ���ʉ��Đ�
            AudioSource.PlayClipAtPoint(clipHit, transform.position);
            // �������t���O
            isProcessed = true;
            // �F�ύX
            objBegin.GetComponent<SpriteRenderer>().color = processedColorEdges;
            objEnd.GetComponent<SpriteRenderer>().color = processedColorEdges;
            objTrail.GetComponent<SpriteRenderer>().color = processedColorTrail;
        }
    }

    public override void OnKeyUp(JudgementType judgementType) {
        // �R���\�[���ɔ���\��
        Debug.Log(judgementType);

        // Bad�ȓ�
        if (judgementType != JudgementType.Poor) {
            EvaluationManager.OnHit(judgementType);
        } else {
            EvaluationManager.OnMiss();
        }

        // ���ʉ��Đ�
        AudioSource.PlayClipAtPoint(clipHit, transform.position);

        // �������t���O����
        isProcessed = false;

        // �폜
        PlayerController.ExistingNoteControllers.Remove(
                GetComponent<NoteControllerBase>());
        Destroy(gameObject);
    }
}
