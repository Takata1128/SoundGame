using System;
using System.Collections.Generic;

public class BMSHeader : IComparable<BMSHeader>
{
    public string ParentPath { get; set; } = "";
    public string Path { get; set; } = "";

    public int Rank { get; set; } = 0;
    public int Level { get; set; } = 0;
    public int Player { get; set; } = 0;

    public string Artist { get; set; } = "";

    public string Genre { get; set; } = "";

    public string Title { get; set; } = "";

    public float Total { get; set; } = 400f;

    // Header情報ではないが持たせる
    public int NoteCount { get; set; } = 0;

    public float MaxBpm { get; set; } = 0f;

    public float MinBpm { get; set; } = 0f;


    public Dictionary<int, string> SoundPathes { get; set; }

    public int CompareTo(BMSHeader h)
    {
        if (Level > h.Level) return 1;
        else if (Level == h.Level) return 0;
        else return -1;
    }
}