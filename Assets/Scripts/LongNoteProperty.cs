using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteProperty : NoteControllerBase
{
    [SerializeField] GameObject objBegin; // 始点部分のGameObject
    [SerializeField] GameObject objTrail; // 軌跡部分のGameObject
    [SerializeField] GameObject objEnd; // 終点部分のGameObject

    [SerializeField] Color32 processedColorEdges; // 処理中の始点終点の色
    [SerializeField] Color32 processedColorTrail; // 処理中の軌跡の色

    [SerializeField] AudioClip clipHit;

    // Update is called once per frame
    void Update() {
        SetTransform();
        CheckMiss();
    }

    private void SetTransform() {

        // 始点の座標
        Vector3 positionBegin = new Vector3();
        positionBegin.x = (float)noteProperty.lane - 3.5f;
        float zBegin = (noteProperty.beatBegin - PlayerController.CurrentBeat) * PlayerController.ScrollSpeed;
        positionBegin.y = 0.0f;
        positionBegin.z = isProcessed ? objBegin.transform.position.z : zBegin;
        objBegin.transform.localPosition = positionBegin;

        // 終点の座標
        Vector3 positionEnd = new Vector3();
        positionEnd.x = (float)noteProperty.lane - 3.5f;
        float zEnd = (noteProperty.beatEnd - PlayerController.CurrentBeat) * PlayerController.ScrollSpeed;
        positionEnd.y = 0.0f;
        positionEnd.z = zEnd;
        objEnd.transform.localPosition = positionEnd;


        // 軌跡部分の座標（始点と終点の中心）
        Vector3 positionTrail = (positionBegin + positionEnd) / 2.0f;
        objTrail.transform.localPosition = positionTrail;


        // 軌跡部分の拡大率
        Vector3 scale = objTrail.transform.localScale;
        scale.z = Vector3.Distance(objBegin.transform.position, objEnd.transform.position);
        objTrail.transform.localScale = scale;

    }

    private void CheckMiss() {
        // 処理中でない && 始点がBAD判定幅を超える
        if (isProcessed && noteProperty.secBegin - PlayerController.CurrentSec < -JudgementManager.JudgementWidth[JudgementType.Bad]) {
            // ミス処理
            EvaluationManager.OnMiss(); // 始点
            EvaluationManager.OnMiss(); // 終点

            // 削除
            PlayerController.ExistingNoteControllers.Remove(
                GetComponent<NoteControllerBase>());
            Destroy(gameObject);
        }

        // 処理中 && 終点がBAD判定幅を超える
        if (isProcessed && noteProperty.secEnd - PlayerController.CurrentSec < -JudgementManager.JudgementWidth[JudgementType.Bad]) {
            // ミス処理
            EvaluationManager.OnMiss();
            // 処理中フラグ解除
            isProcessed = false;

            // 削除
            PlayerController.ExistingNoteControllers.Remove(
                GetComponent<NoteControllerBase>());
            Destroy(gameObject);
        }
    }

    public override void OnKeyDown(JudgementType judgementType) {
        // コンソールに判定表示
        Debug.Log(judgementType);

        // 判定がPoorでない
        if (judgementType != JudgementType.Poor) {
            // ヒット処理
            EvaluationManager.OnHit(judgementType);
            // 効果音再生
            AudioSource.PlayClipAtPoint(clipHit, transform.position);
            // 処理中フラグ
            isProcessed = true;
            // 色変更
            objBegin.GetComponent<SpriteRenderer>().color = processedColorEdges;
            objEnd.GetComponent<SpriteRenderer>().color = processedColorEdges;
            objTrail.GetComponent<SpriteRenderer>().color = processedColorTrail;
        }
    }

    public override void OnKeyUp(JudgementType judgementType) {
        // コンソールに判定表示
        Debug.Log(judgementType);

        // Bad以内
        if (judgementType != JudgementType.Poor) {
            EvaluationManager.OnHit(judgementType);
        } else {
            EvaluationManager.OnMiss();
        }

        // 効果音再生
        AudioSource.PlayClipAtPoint(clipHit, transform.position);

        // 処理中フラグ解除
        isProcessed = false;

        // 削除
        PlayerController.ExistingNoteControllers.Remove(
                GetComponent<NoteControllerBase>());
        Destroy(gameObject);
    }
}
