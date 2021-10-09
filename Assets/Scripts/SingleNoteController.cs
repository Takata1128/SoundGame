using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleNoteController : NoteControllerBase
{
    [SerializeField] AudioClip clipHit; // 効果音

    // Update is called once per frame
    void Update() {
        SetTransform();
        CheckMiss();
    }


    // 座標設定
    private void SetTransform() {
        Vector3 position = new Vector3();
        position.x = (float)noteProperty.lane - 3.5f;
        position.z = (noteProperty.beatBegin - PlayerController.CurrentBeat) * PlayerController.ScrollSpeed;
        position.y = 0.0f;

        transform.localPosition = position;
    }

    // 見逃し検出
    private void CheckMiss() {
        // Badの判定幅を通過
        if (noteProperty.secBegin - PlayerController.CurrentSec < -JudgementManager.JudgementWidth[JudgementType.Bad]) {
            // ミス処理
            EvaluationManager.OnMiss();

            // 未処理ノーツ一覧から削除
            PlayerController.ExistingNoteControllers.Remove(
                GetComponent<NoteControllerBase>()
                );
            // GameObject自体も削除
            Destroy(gameObject);
        }
    }

    public override void OnKeyDown(JudgementType judgementType) {
        // デバッグ用にコンソールに判定を出力
        Debug.Log(judgementType);

        if (judgementType != JudgementType.Poor) {
            // ヒット処理
            EvaluationManager.OnHit(judgementType);

            // 効果音再生
            AudioSource.PlayClipAtPoint(clipHit, transform.position);
            // 未処理ノーツ一覧から削除
            PlayerController.ExistingNoteControllers.Remove(
                GetComponent<NoteControllerBase>());
            Destroy(gameObject);
        }

    }
}
