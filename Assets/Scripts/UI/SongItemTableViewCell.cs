using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongItemData
{
    public string SongTitle { get; }
    public string ComposerName { get; }

    public SongItemData(string songTitle, string composerName)
    {
        SongTitle = songTitle;
        ComposerName = composerName;
    }
}

public class SongItemTableViewCell : TableViewCell<SongItemData>
{
    [SerializeField] private TextMeshProUGUI songTitleLabel;
    [SerializeField] private TextMeshProUGUI composerNameLabel;

    public override void UpdateContent(SongItemData itemData)
    {
        songTitleLabel.text = itemData.SongTitle;
        composerNameLabel.text = itemData.ComposerName;
    }
}