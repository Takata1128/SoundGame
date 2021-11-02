using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement; // �V�[���J�ڂɕK�v
using UnityEngine.UI;

public class SelectorController : MonoBehaviour
{
    [SerializeField] private Text textInformation;
    [SerializeField] private Text textScrollSpeed;

    // �t�H�[�}�b�g�w�蕶����
    private string informationTextFormat;
    private string scrollSpeedTextFormat;

    // �X�N���[�����x
    private float scrollSpeed = 5.0f;

    // BMS�t�@�C���ꗗ
    private string[] beatmapPaths;
    public static List<BmsLoader> BmsLoaders;
    private BmsLoader selectedBmsLoader;

    // �I�𒆂̕���ID
    private int selectedIndex = 0;

    // ���ʂ̐�
    private int beatmapCount;


    // Start is called before the first frame update
    void Start()
    {
        if (BmsLoaders == null)
        {
            LoadBeatScores();
        }

        selectedBmsLoader = BmsLoaders[selectedIndex];
        beatmapCount = BmsLoaders.Count();

        // ������Ԃ̃e�L�X�g���e
        informationTextFormat = textInformation.text;
        scrollSpeedTextFormat = textScrollSpeed.text;

        // ������ԂŃe�L�X�g�X�V
        ChangeSelectedIndex(selectedIndex);
        ChangeScrollSpeed(scrollSpeed);
    }

    // 譜面の読み込み
    void LoadBeatScores()
    {
        // �t�H���_�p�X
        var beatmapDirectory = Application.dataPath + "/../Beatmaps";
        // BMS�t�@�C���ꗗ�擾
        beatmapPaths = Directory.GetFiles(beatmapDirectory, "*.bm?", SearchOption.AllDirectories);
        // ���ʏ��ǂݍ���
        BmsLoaders = new List<BmsLoader>();
        for (int i = 0; i < beatmapPaths.Length; i++)
        {
            // �g���q�I��
            string ext = Path.GetExtension(beatmapPaths[i]);
            if (ext == ".bms" || ext == ".bme")
            {
                try
                {
                    BmsLoaders.Add(new BmsLoader(beatmapPaths[i]));
                }
                catch (KeyNotFoundException e)
                {
                    Debug.Log(beatmapPaths[i] + " �̓ǂݍ��݂ŃG���[���������܂����B");
                    Debug.Log(e);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ����ID�ύX
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeSelectedIndex(selectedIndex - 1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeSelectedIndex(selectedIndex + 1);
        }

        // �X�N���[�����x�ύX
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeScrollSpeed(scrollSpeed + 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeScrollSpeed(scrollSpeed - 0.1f);
        }

        // ���菈��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerController.ScrollSpeed = scrollSpeed;
            PlayerController.BmsHeader = selectedBmsLoader.BmsHeader;
            PlayerController.BmsScore = selectedBmsLoader.BmsScore;
            PlayerController.BmsLoaders = BmsLoaders;
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


    private void ChangeSelectedIndex(int newIndex)
    {
        selectedIndex = Mathf.Clamp(newIndex, 0, beatmapCount - 1);
        selectedBmsLoader = BmsLoaders[selectedIndex];

        // �y�ȏ��
        var title = selectedBmsLoader.BmsHeader.Title;
        var artist = selectedBmsLoader.BmsHeader.Artist;
        var playLevel = selectedBmsLoader.BmsHeader.Artist;
        var notesCount = selectedBmsLoader.BmsScore.NoteCount;

        var minBpm = selectedBmsLoader.BmsScore.Bpms.Min(x => x.Bpm);
        var maxBpm = selectedBmsLoader.BmsScore.Bpms.Max(x => x.Bpm);

        // �e�L�X�g�ύX
        var text = string.Format(informationTextFormat,
            title, artist, playLevel, notesCount, minBpm, maxBpm);
        textInformation.text = text;

    }

    private void ChangeScrollSpeed(float newScrollSpeed)
    {
        scrollSpeed = Mathf.Clamp(newScrollSpeed, 0.1f, 10f);

        var text = string.Format(scrollSpeedTextFormat, scrollSpeed);
        textScrollSpeed.text = text;
    }
}
