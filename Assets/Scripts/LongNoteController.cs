using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LongNoteController : NoteControllerBase
{
    new LongNote Note { get; set; }
    [SerializeField] GameObject objBegin; // �n�_������GameObject
    [SerializeField] GameObject objTrail; // �O�Օ�����GameObject
    [SerializeField] GameObject objEnd; // �I�_������GameObject

    [SerializeField] Color32 processedColorEdges; // �������̎n�_�I�_�̐F
    [SerializeField] Color32 processedColorTrail; // �������̋O�Ղ̐F

    private void Start()
    {
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        SetTransform();
    }

    private void SetTransform()
    {
        // �n�_�̍��W
        Vector3 positionBegin = new Vector3();
        positionBegin.x = (float)Lane - 4.5f;
        float zBegin = (Note.BeatBegin - PlayerController.CurrentBeat) * PlayerController.ScrollSpeed;
        positionBegin.y = 0.0f;
        positionBegin.z = IsProcessed ? objBegin.transform.position.z : zBegin;
        objBegin.transform.localPosition = positionBegin;

        // �I�_�̍��W
        Vector3 positionEnd = new Vector3();
        positionEnd.x = (float)Lane - 4.5f;
        float zEnd = (Note.BeatEnd - PlayerController.CurrentBeat) * PlayerController.ScrollSpeed;
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

    public override bool CheckMiss()
    {

        // �������łȂ� && �n�_��BAD���蕝�𒴂���
        if (IsProcessed && Note.SecBegin - PlayerController.CurrentSec < -JudgementManager.JudgementWidth[JudgementType.Bad])
        {
            // �~�X����
            EvaluationManager.OnMiss(); // �n�_
            EvaluationManager.OnMiss(); // �I�_
            Destroy(gameObject);
            return true;
        }

        // ������ && �I�_��BAD���蕝�𒴂���
        if (IsProcessed && Note.SecEnd - PlayerController.CurrentSec < -JudgementManager.JudgementWidth[JudgementType.Bad])
        {
            // �~�X����
            EvaluationManager.OnMiss();
            // �������t���O����
            IsProcessed = false;
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    public override void OnKeyDown(JudgementType judgementType)
    {
        // �R���\�[���ɔ���\��
        Debug.Log(judgementType);

        if (judgementType != JudgementType.Poor)
        {
            // �q�b�g����
            EvaluationManager.OnHit(judgementType);
            // �������t���O
            IsProcessed = true;
            // �F�ύX
            objBegin.GetComponent<SpriteRenderer>().color = processedColorEdges;
            objEnd.GetComponent<SpriteRenderer>().color = processedColorEdges;
            objTrail.GetComponent<SpriteRenderer>().color = processedColorTrail;

            Vector3 pos = gameObject.transform.position;
            var burstEffect = Instantiate(prefabBurst, pos, Quaternion.identity);
            burstEffect.transform.localScale *= burstSize;
            Destroy(burstEffect, deleteTime);
        }
    }

    public override void OnKeyUp(JudgementType judgementType)
    {
        // �R���\�[���ɔ���\��
        Debug.Log(judgementType);

        // Bad�ȓ�
        if (judgementType != JudgementType.Poor)
        {
            EvaluationManager.OnHit(judgementType);

            Vector3 pos = gameObject.transform.position;
            var burstEffect = Instantiate(prefabBurst, pos, Quaternion.identity);
            burstEffect.transform.localScale *= burstSize;
            Destroy(burstEffect, deleteTime);
        }
        else
        {
            EvaluationManager.OnMiss();
        }

        // �������t���O����
        IsProcessed = false;

        Destroy(gameObject);
    }
}
