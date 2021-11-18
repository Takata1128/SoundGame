using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectorController : MonoBehaviour
{
    // [SerializeField] private Text textInformation;
    [SerializeField] private TextMeshProUGUI textScrollSpeed;

    // �t�H�[�}�b�g�w�蕶����
    // private string informationTextFormat;
    private string scrollSpeedTextFormat;

    // �X�N���[�����x

    private GameObject tableViewObject;

    private SongItemTableViewController tableViewController;

    // BMS�t�@�C���ꗗ
    private BmsData selectedBmsData;

    // �I�𒆂̕���ID
    private int selectedIndex = 0;

    // ���ʂ̐�
    private int beatmapCount;


    // Start is called before the first frame update
    void Start()
    {
        selectedBmsData = BmsDataCenter.CachedBmsDataList[selectedIndex];
        beatmapCount = BmsDataCenter.CachedBmsDataList.Count();

        scrollSpeedTextFormat = textScrollSpeed.text;
        ChangeScrollSpeed(BmsDataCenter.ScrollSpeed);
        tableViewObject = GameObject.Find("Table View");
        tableViewController = tableViewObject.GetComponent<SongItemTableViewController>();
        tableViewController.LoadData(BmsDataCenter.CachedBmsDataList);
    }



    // Update is called once per frame
    void Update()
    {
        // �X�N���[�����x�ύX
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeScrollSpeed(BmsDataCenter.ScrollSpeed + 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeScrollSpeed(BmsDataCenter.ScrollSpeed - 0.1f);
        }

        // ���菈��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerController.ScrollSpeed = BmsDataCenter.ScrollSpeed;
            PlayerController.BmsData = BmsDataCenter.CachedBmsDataList[tableViewController.SelectedIndex];
            SceneManager.LoadScene("PlayScene");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
#endif
        }
    }

    private void ChangeScrollSpeed(float newScrollSpeed)
    {
        BmsDataCenter.ScrollSpeed = Mathf.Clamp(newScrollSpeed, 0.1f, 10f);

        var text = string.Format(scrollSpeedTextFormat, BmsDataCenter.ScrollSpeed);
        textScrollSpeed.text = text;
    }
}
