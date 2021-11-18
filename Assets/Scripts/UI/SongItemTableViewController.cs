using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


[RequireComponent(typeof(ScrollRect))]
public class SongItemTableViewController : TableViewController<SongItemData>
{
    [SerializeField] private TextMeshProUGUI songTitleText;
    [SerializeField] private TextMeshProUGUI genreText;
    [SerializeField] private TextMeshProUGUI artistText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI noteCountText;
    [SerializeField] private TextMeshProUGUI bpmText;


    private string songTitleTextFormat;
    private string genreTextFormat;
    private string artistTextFormat;
    private string levelTextFormat;
    private string noteCountTextFormat;
    private string bpmTextFormat;


    public int SelectedIndex { get; set; } = 0;

    private List<BMSHeader> bmsHeaderCache = new List<BMSHeader>();

    public void LoadData(List<BmsData> bmsDataList)
    {
        tableData = new List<SongItemData>();
        foreach (BmsData bmsData in bmsDataList)
        {
            tableData.Add(new SongItemData(
                bmsData.BmsHeader.Title,
                bmsData.BmsHeader.Artist
            ));
            bmsHeaderCache.Add(
                bmsData.BmsHeader
            );
        }

        // スクロールさせる内容のサイズ更新
        UpdateContentSize();
        UpdateContents();
    }

    protected override float CellHeightAtIndex(int index)
    {
        if (index == SelectedIndex)
        {
            return 96.0f;
        }
        return 64.0f;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        songTitleTextFormat = songTitleText.text;
        genreTextFormat = genreText.text;
        artistTextFormat = artistText.text;
        levelTextFormat = levelText.text;
        noteCountTextFormat = noteCountText.text;
        bpmTextFormat = bpmText.text;
        UpdateSongDetail(SelectedIndex);
    }

    public void OnPressCell(SongItemTableViewCell cell)
    {
        SelectedIndex = cell.DataIndex;
        UpdateSongDetail(SelectedIndex);
    }

    private void UpdateSongDetail(int index)
    {
        BMSHeader header = bmsHeaderCache[index];
        songTitleText.text = string.Format(songTitleTextFormat,
            header.Title);
        genreText.text = string.Format(genreTextFormat,
            header.Genre);
        artistText.text = string.Format(artistTextFormat,
                    header.Artist);
        levelText.text = string.Format(levelTextFormat,
                    header.Level);
        noteCountText.text = string.Format(noteCountTextFormat,
                    header.NoteCount);
        bpmText.text = string.Format(bpmTextFormat,
                    header.MaxBpm, header.MinBpm);
    }
}