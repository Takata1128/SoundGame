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
    private List<BmsLoader> bmsLoaders;
    private BmsLoader selectedBmsLoader;

    // �I�𒆂̕���ID
    private int selectedIndex = 0;

    // ���ʂ̐�
    private int beatmapCount;


    // Start is called before the first frame update
    void Start()
    {
        // �t�H���_�p�X
        var beatmapDirectory = Application.dataPath + "/../Beatmaps";
        // BMS�t�@�C���ꗗ�擾
        beatmapPaths = Directory.GetFiles(beatmapDirectory, "*.bm?", SearchOption.AllDirectories);


        // ���ʏ��ǂݍ���
        bmsLoaders = new List<BmsLoader>();
        for (int i = 0; i < beatmapPaths.Length; i++)
        {
            // �g���q�I��
            string ext = Path.GetExtension(beatmapPaths[i]);
            if (ext == ".bms" || ext == ".bme")
            {
                try
                {
                    bmsLoaders.Add(new BmsLoader(beatmapPaths[i]));
                }
                catch (KeyNotFoundException e)
                {
                    Debug.Log(beatmapPaths[i] + " �̓ǂݍ��݂ŃG���[���������܂����B");
                    Debug.Log(e);
                }
            }
        }



        selectedBmsLoader = bmsLoaders[selectedIndex];
        beatmapCount = bmsLoaders.Count();

        // ������Ԃ̃e�L�X�g���e
        informationTextFormat = textInformation.text;
        scrollSpeedTextFormat = textScrollSpeed.text;

        // ������ԂŃe�L�X�g�X�V
        ChangeSelectedIndex(selectedIndex);
        ChangeScrollSpeed(scrollSpeed);
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
            PlayerController.beatmap = new Beatmap(beatmapPaths[selectedIndex]);
            SceneManager.LoadScene("PlayScene");
        }
    }


    private void ChangeSelectedIndex(int newIndex)
    {
        selectedIndex = Mathf.Clamp(newIndex, 0, beatmapCount - 1);
        selectedBmsLoader = bmsLoaders[selectedIndex];

        // �y�ȏ��
        var title = selectedBmsLoader.headerData["TITLE"];
        var artist = selectedBmsLoader.headerData["ARTIST"];
        var playLevel = selectedBmsLoader.headerData["PLAYLEVEL"];
        var notesCount = selectedBmsLoader.noteProperties.Count(x => x.noteType == NoteType.Single) +
            selectedBmsLoader.noteProperties.Count(x => x.noteType == NoteType.Long) * 2;

        var minBpm = selectedBmsLoader.tempoChanges.Min(x => x.tempo);
        var maxBpm = selectedBmsLoader.tempoChanges.Max(x => x.tempo);

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
