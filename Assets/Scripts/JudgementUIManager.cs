using UnityEngine;
using UnityEngine.UI;

public class JudgementUIManager : MonoBehaviour
{
    [SerializeField] private Text judgementValue;

    [SerializeField] private GameObject judgementTextObject;

    [SerializeField] private Color judgementPerfectColor;

    [SerializeField] private Color judgementGreatColor;

    [SerializeField] private Color judgementGoodColor;

    [SerializeField] private Color judgementBadColor;

    [SerializeField] private Color judgementPoorColor;


    private string judgeValueFormat;

    private float lastJudgeSec = 0.0f;
    private void Start()
    {

        judgeValueFormat = judgementValue.text;
        judgementTextObject.SetActive(false);
    }

    private void Update()
    {
        CheckJudgementText();
    }

    public void ShowJudge(JudgementType judgement)
    {
        if (!judgementTextObject.activeSelf)
            judgementTextObject.SetActive(true);
        if (judgement == JudgementType.Poor)
        {
            judgementValue.text = string.Format(
                judgeValueFormat,
                judgement.ToString(),
                ""
            );
        }
        else
        {
            judgementValue.text = string.Format(
                judgeValueFormat,
                judgement.ToString(),
                EvaluationManager.combo
            );
        }
        switch (judgement)
        {
            case JudgementType.Perfect:
                judgementValue.color = judgementPerfectColor;
                break;
            case JudgementType.Great:
                judgementValue.color = judgementGreatColor;
                break;
            case JudgementType.Good:
                judgementValue.color = judgementGoodColor;
                break;
            case JudgementType.Bad:
                judgementValue.color = judgementBadColor;
                break;
            case JudgementType.Poor:
                judgementValue.color = judgementPoorColor;
                break;
        }
        lastJudgeSec = PlayerController.CurrentSec;
    }

    private void CheckJudgementText()
    {
        if (judgementTextObject.activeSelf && PlayerController.CurrentSec - lastJudgeSec > 1.0f)
        {
            judgementTextObject.SetActive(false);
        }
    }
}