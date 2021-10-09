using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationManager : MonoBehaviour
{
    // スコア、コンボ
    public static int score;
    public static int combo;
    public static int maxCombo;

    // 判定の個数
    public static Dictionary<JudgementType, int> judgementCounts = new Dictionary<JudgementType, int>();

    // 判定のスコア増加量
    public static Dictionary<JudgementType, int> judgementScores = new Dictionary<JudgementType, int>() {
        {JudgementType.Perfect,2},
        {JudgementType.Great,1},
        {JudgementType.Good,0},
        {JudgementType.Bad,0},
        {JudgementType.Poor,0},
};


    // Start is called before the first frame update
    void Start() {
        score = 0;
        combo = 0;
        maxCombo = 0;

        judgementCounts[JudgementType.Perfect] = 0;
        judgementCounts[JudgementType.Great] = 0;
        judgementCounts[JudgementType.Good] = 0;
        judgementCounts[JudgementType.Bad] = 0;
        judgementCounts[JudgementType.Poor] = 0;
    }

    // Update is called once per frame
    void Update() {
        // maxCombo更新
        maxCombo = Mathf.Max(combo, maxCombo);
    }

    public static void OnHit(JudgementType judgementType) {
        // コンボ数をインクリメント
        combo++;

        // スコア加算
        score += judgementScores[judgementType];

        // カウントインクリメント
        judgementCounts[judgementType]++;
    }

    public static void OnMiss() {
        // コンボリセット
        combo = 0;
        // 見逃しインクリメント
        judgementCounts[JudgementType.Poor]++;
    }
}
