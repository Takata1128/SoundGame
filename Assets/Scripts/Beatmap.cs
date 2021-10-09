using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public class Beatmap
{
    public List<NoteProperty> noteProperties = new List<NoteProperty>();
    public List<BgmProperty> bgmProperties = new List<BgmProperty>();
    public List<TempoChange> tempoChanges = new List<TempoChange>();
    public Dictionary<string, string> audioFilePaths = new Dictionary<string, string>();


    public Beatmap(string filePath)
    {
        var bmsDirectory = new FileInfo(filePath).DirectoryName;
        var bmsLoader = new BmsLoader(filePath);

        noteProperties = bmsLoader.noteProperties;
        bgmProperties = bmsLoader.bgmProperties;
        tempoChanges = bmsLoader.tempoChanges;

        foreach (KeyValuePair<string, string> item in bmsLoader.headerData)
        {
            Match match = Regex.Match(item.Key, @"WAV([0-9A-Z]{2})");
            if (match.Success)
            {
                string index = match.Groups[1].Value;
                audioFilePaths[index] = bmsDirectory + "\\" + item.Value;
            }
        }
    }

    public static float ToSecWithFixedTempo(float beat, float tempo)
    {
        return beat / (tempo / 60f);
    }

    public static float ToBeatWithFixedTempo(float sec, float tempo)
    {
        return sec * (tempo / 60f);
    }

    public static float ToSec(float beat, List<TempoChange> tempoChanges)
    {
        float accumulatedSec = 0f;
        int i = 0;
        var n = tempoChanges.Count(x => x.beat <= beat);

        while (i < n - 1)
        {
            accumulatedSec += ToSecWithFixedTempo(
                tempoChanges[i + 1].beat - tempoChanges[i].beat,
                    tempoChanges[i].tempo
                );
            i++;
        }
        accumulatedSec += ToSecWithFixedTempo(
            beat - tempoChanges[i].beat, tempoChanges[i].tempo);
        return accumulatedSec;
    }

    public static float ToBeat(float sec, List<TempoChange> tempoChanges)
    {
        float accumulatedSec = 0f;
        int i = 0;
        var n = tempoChanges.Count;

        while (i < n - 1)
        {
            var tmpSec = accumulatedSec;
            accumulatedSec += ToSecWithFixedTempo(
                tempoChanges[i + 1].beat - tempoChanges[i].beat,
                    tempoChanges[i].tempo
                );
            if (accumulatedSec >= sec)
            {
                return tempoChanges[i].beat + ToBeatWithFixedTempo(sec - tmpSec, tempoChanges[i].tempo);
            }
            i++;
        }
        return tempoChanges[n - 1].beat + ToBeatWithFixedTempo(sec - accumulatedSec, tempoChanges[n - 1].tempo);
    }
}
