using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationManager : MonoBehaviour
{
    public static int score;
    public static int combo;
    public static int maxCombo;

    public static Dictionary<JudgementType, int> judgementCounts = new Dictionary<JudgementType, int>();

    public static Dictionary<JudgementType, int> judgementScores = new Dictionary<JudgementType, int>() {
        {JudgementType.Perfect,2},
        {JudgementType.Great,1},
        {JudgementType.Good,0},
        {JudgementType.Bad,0},
        {JudgementType.Poor,0},
};


    // Start is called before the first frame update
    void Start()
    {
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
    void Update()
    {
        // maxCombo�X�V
        maxCombo = Mathf.Max(combo, maxCombo);
    }

    public static void OnHit(JudgementType judgementType)
    {
        // �R���{�����C���N�������g
        combo++;

        // �X�R�A���Z
        score += judgementScores[judgementType];

        // �J�E���g�C���N�������g
        judgementCounts[judgementType]++;
    }

    public static void OnMiss()
    {
        // �R���{���Z�b�g
        combo = 0;
        // �������C���N�������g
        judgementCounts[JudgementType.Poor]++;
    }
}
