using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text textValue;
    private string valueFormat;

    // Start is called before the first frame update
    void Start() {
        valueFormat = textValue.text;
    }

    // Update is called once per frame
    void Update() {
        textValue.text = string.Format(valueFormat,
            // スコア
            EvaluationManager.score,
            // コンボ数
            EvaluationManager.combo,
            // 最大コンボ数
            EvaluationManager.maxCombo,
            // PERFECT
            EvaluationManager.judgementCounts[JudgementType.Perfect],
            // GREAT
            EvaluationManager.judgementCounts[JudgementType.Great],
            // GOOD
            EvaluationManager.judgementCounts[JudgementType.Good],
            // BAD
            EvaluationManager.judgementCounts[JudgementType.Bad],
            // POOR
            EvaluationManager.judgementCounts[JudgementType.Poor]
        );
    }
}
