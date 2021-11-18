using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class ResultController : MonoBehaviour
{
    // [SerializeField] private Text textInformation;
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private TextMeshProUGUI textPerfect;
    [SerializeField] private TextMeshProUGUI textGreat;
    [SerializeField] private TextMeshProUGUI textGood;
    [SerializeField] private TextMeshProUGUI textBad;
    [SerializeField] private TextMeshProUGUI textPoor;



    private string scoreTextFormat;
    private string prefectTextFormat;
    private string greatTextFormat;
    private string goodTextFormat;
    private string badTextFormat;
    private string poorTextFormat;

    public Dictionary<JudgementType, int> JudgementCounts { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        // TODO ちゃんとスコア渡す
        textScore.text = string.Format(textScore.text,
            JudgementCounts[JudgementType.Perfect] * 2 + JudgementCounts[JudgementType.Great]);
        textPerfect.text = string.Format(textPerfect.text,
            JudgementCounts[JudgementType.Perfect]);
        textGreat.text = string.Format(textGreat.text,
                    JudgementCounts[JudgementType.Great]);
        textGood.text = string.Format(textGood.text,
                    JudgementCounts[JudgementType.Good]);
        textBad.text = string.Format(textBad.text,
                    JudgementCounts[JudgementType.Bad]);
        textPoor.text = string.Format(textPoor.text,
                    JudgementCounts[JudgementType.Poor]);
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("SelectScene");
        }
    }
}
