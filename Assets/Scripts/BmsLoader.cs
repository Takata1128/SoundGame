using System.Collections;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class BmsLoader
{
    private static string mainDataPattern = @"#([0-9]{3})([0-9A-Z]{2}):(.*)";

    private static List<string> headerPatterns = new List<string> {
        @"#(PLAYER) (.*)",
        @"#(GENRE) (.*)",
        @"#(TITLE) (.*)",
        @"#(TOTAL) (.*)",
        @"#(ARTIST) (.*)",
        @"#(BPM) (.*)",
        @"#(BPM[0-9A-Z]{2}) (.*)",
        @"#(PLAYLEVEL) (.*)",
        @"#(RANK) (.*)"
    };

    private static string soundPattern =
        @"#WAV([0-9A-Z]{2}) (.*)"; // #WAV00~ZZ

    private static Dictionary<char, int> lanePairs = new Dictionary<char, int>(){
        {'1',1},
        {'2',2},
        {'3',3},
        {'4',4},
        {'5',5},
        {'6',8},
        {'8',6},
        {'9',7},
    };

    // �w�b�_�[���A�f�[�^


    // (�R���X�g���N�^) BMS�t�@�C����ǂݍ���
    public static BmsData Load(string filePath)
    {
        Dictionary<string, string> headerData;
        Dictionary<int, string> soundPathes;
        BMSHeader BmsHeader = new BMSHeader();
        BMSScore BmsScore = new BMSScore();
        headerData = new Dictionary<string, string>() {
            { "PLAYER","NULL"},
            { "TITLE","NULL"},
            { "ARTIST","NULL"},
            { "PLAYLEVEL","NULL"},
            { "RANK","NULL"},
            { "BPM","0"}
        };
        soundPathes = new Dictionary<int, string>();

        BmsHeader.Path = filePath;
        BmsHeader.ParentPath = System.IO.Path.GetDirectoryName(filePath);
        // BMS�t�@�C����ǂݍ��݁A�e�s��ێ�
        var lines = File.ReadAllLines(filePath, Encoding.UTF8);

        foreach (var line in lines)
        {
            LoadHeaderLine(line, ref headerData, soundPathes);
        }

        SetBmsHeader(ref BmsHeader, headerData);
        BmsHeader.SoundPathes = soundPathes;
        BmsScore.AddBPM(
            new BPM(Convert.ToSingle(headerData["BPM"]), 0f)
        );
        foreach (var line in lines)
        {
            LoadMainDataLine(line, BmsHeader, ref BmsScore, headerData);
        }
        // ノーツソート
        foreach (Lane lane in BmsScore.Lanes)
        {
            lane.NoteList = lane.NoteList.OrderBy(x => -x.BeatBegin).ToList();
        }
        // BGMソート
        BmsScore.BGSounds = BmsScore.BGSounds.OrderBy(x => -x.BeatBegin).ToList();
        // Bpmソート
        BmsScore.Bpms = BmsScore.Bpms.OrderBy(x => x.BeatBegin).ToList();
        BmsScore.SetNotesSec();
        BmsScore.Bpms = BmsScore.Bpms.OrderBy(x => -x.BeatBegin).ToList();

        // Score情報を Headerにセット
        BmsHeader.NoteCount = BmsScore.NoteCount;
        BmsHeader.MinBpm = BmsScore.Bpms.Min(x => x.Bpm);
        BmsHeader.MaxBpm = BmsScore.Bpms.Max(x => x.Bpm);
        return new BmsData(BmsHeader, BmsScore);
    }

    // �w�b�_�[�s�̂ݓǂݍ���
    private static void LoadHeaderLine(string line, ref Dictionary<string, string> headerData, Dictionary<int, string> soundPathes)
    {
        Match wavMatch = Regex.Match(line, soundPattern);

        if (wavMatch.Success)
        {
            var index = Util.Decode(wavMatch.Groups[1].Value);
            var path = wavMatch.Groups[2].Value;
            soundPathes[index] = path;
        }
        else
        {
            foreach (var headerPattern in headerPatterns)
            {
                Match match = Regex.Match(line, headerPattern);
                if (match.Success)
                {
                    // �w�b�_�[��
                    var headerName = match.Groups[1].Value;
                    // �f�[�^�{��
                    var data = match.Groups[2].Value;
                    headerData[headerName] = data;
                    Debug.Log(headerName + ": " + data);
                    return;
                }
            }
        }


    }

    private static void SetBmsHeader(ref BMSHeader BmsHeader, Dictionary<string, string> headerData)
    {
        // Initialize BmsHeader
        if (headerData.ContainsKey("PLAYER"))
        {
            BmsHeader.Player = Int32.Parse(headerData["PLAYER"]);
        }
        if (headerData.ContainsKey("ARTIST"))
        {
            BmsHeader.Artist = headerData["ARTIST"];
        }
        if (headerData.ContainsKey("PLAYLEVEL"))
        {
            int data;
            int.TryParse(headerData["PLAYLEVEL"], out data);
            BmsHeader.Level = data;
        }
        if (headerData.ContainsKey("GENRE"))
        {
            BmsHeader.Genre = headerData["GENRE"];
        }
        if (headerData.ContainsKey("RANK"))
        {
            int data;
            int.TryParse(headerData["RANK"], out data);
            BmsHeader.Rank = data;
        }
        if (headerData.ContainsKey("TITLE"))
        {
            BmsHeader.Title = headerData["TITLE"];
        }
        if (headerData.ContainsKey("TOTAL"))
        {
            BmsHeader.Total = Int32.Parse(headerData["TOTAL"]);
        }
    }

    // ���C���f�[�^�s�̂ݓǂݍ���
    private static void LoadMainDataLine(string line, BMSHeader bMSHeader, ref BMSScore bMSScore, Dictionary<string, string> headerData)
    {
        float[] measureLength = Enumerable.Repeat(4f, 1000).ToArray();
        float[] longNoteBeginBuffers = new float[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        int[] longNoteSoundBuffers = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        var match = Regex.Match(line, mainDataPattern);

        if (match.Success)
        {
            // ���ߔԍ�
            int measureNum = Convert.ToInt32(match.Groups[1].Value);
            // �`�����l���ԍ�
            string channel = match.Groups[2].Value;
            // �f�[�^�{��
            string body = match.Groups[3].Value;
            // �f�[�^���
            DataType dataType = GetDataType(channel);

            // �Ή��\�ȃf�[�^�łȂ��ꍇ
            if (dataType == DataType.Unsupported)
            {
                return;
            }

            // ���q�ω�
            if (dataType == DataType.MeasureChange)
            {
                measureLength[measureNum] = Convert.ToSingle(body) * 4f;
            }
            // �m�[�c�ABPM�ω�
            else if (dataType == DataType.SingleNote ||
                dataType == DataType.LongNote ||
                dataType == DataType.DirectTempoChange ||
                dataType == DataType.IndexedTempoChange ||
                dataType == DataType.Bgm)
            {
                // ���߂̊J�nbeat (measureLength�̐擪����measureNum���̍��v)
                float measureStartBeat = measureLength.Take(measureNum).Sum();
                // �I�u�W�F�N�g�̌�
                int objCount = body.Length / 2;
                // �I�u�W�F�N�g���Ƃɏ���
                for (int i = 0; i < objCount; i++)
                {
                    string objNum = body.Substring(i * 2, 2);
                    // �x��
                    if (objNum == "00") continue;

                    // ���݌��Ă���I�u�W�F�N�g��beat
                    float beat = measureStartBeat + (i * measureLength[measureNum] / objCount);

                    // �m�[�c
                    if (dataType == DataType.SingleNote || dataType == DataType.LongNote)
                    {
                        // ���[���ԍ�(�`�����l���ԍ��̈�̈�)
                        if (!lanePairs.ContainsKey(channel[1]))
                        {
                            Debug.Log($"Invalid LaneChannel: {bMSHeader.Title}, channel={channel[1]}");
                            continue;
                        }
                        int lane = lanePairs[channel[1]];

                        switch (dataType)
                        {
                            case DataType.SingleNote: // �V���O���m�[�c
                                bMSScore.AddNote(lane, beat, Util.Decode(objNum));
                                break;
                            case DataType.LongNote: // �����O�m�[�c
                                if (longNoteBeginBuffers[lane] < 0)
                                {
                                    longNoteBeginBuffers[lane] = beat;
                                    longNoteSoundBuffers[lane] = Util.Decode(objNum);
                                }
                                // �����O�m�[�c�t���OON�̂Ƃ�
                                else
                                {
                                    // noteProperties.Add(new NoteProperty(longNoteBeginBuffers[lane], beat, lane, NoteType.Long));
                                    bMSScore.AddLongNote(lane, longNoteBeginBuffers[lane], beat, longNoteSoundBuffers[lane], Util.Decode(objNum));
                                    longNoteBeginBuffers[lane] = -1;
                                    longNoteSoundBuffers[lane] = -1;
                                }
                                break;
                        }
                    }

                    // ���ڎw��^�C�v�̃e���|�ω�
                    else if (dataType == DataType.DirectTempoChange)
                    {
                        // 16�i��BPM��10�i����
                        float bpm = Convert.ToInt32(objNum, 16);
                        bMSScore.AddBPM(beat, bpm);
                    }
                    // �C���f�b�N�X�w��^�C�v�̃e���|�ω�
                    else if (dataType == DataType.IndexedTempoChange)
                    {
                        // headerData��"BPMxx"�Ƃ����L�[�����l�������ɕϊ�
                        float bpm = Convert.ToSingle(headerData["BPM" + objNum]);
                        bMSScore.AddBPM(beat, bpm);
                    }
                    // BGM
                    else if (dataType == DataType.Bgm)
                    {
                        string index = objNum;
                        // bgmProperties.Add(new BgmProperty(beat, index));
                        bMSScore.AddBGSound(beat, Util.Decode(objNum));
                    }
                }
            }
        }
    }

    // �`�����l���ԍ�����f�[�^�̎�ނ����߂�
    private static DataType GetDataType(string channel)
    {
        // �`�����l���̏\�̈ʂ�1�̂Ƃ�
        if (channel[0] == '1')
        {
            // �V���O���m�[�c
            return DataType.SingleNote;
        }
        // �`�����l���̏\�̈ʂ�5�̂Ƃ�
        else if (channel[0] == '5')
        {
            return DataType.LongNote;
        }
        // �`�����l����02
        else if (channel == "02")
        {
            // ���q�ω�
            return DataType.MeasureChange;
        }
        // �`�����l����01
        else if (channel == "01")
        {
            // BGM
            return DataType.Bgm;
        }
        // �`�����l����03
        else if (channel == "03")
        {
            // BPM���ڎw��^�C�v�̃e���|�ω�
            return DataType.DirectTempoChange;
        }
        // �`�����l����08
        else if (channel == "08")
        {
            // BPM�C���f�b�N�X�w��^BPM�ω�
            return DataType.IndexedTempoChange;
        }
        // ����ȊO
        else
        {
            return DataType.Unsupported;
        }

    }

}

public enum DataType
{
    Unsupported,
    SingleNote,
    LongNote,
    DirectTempoChange,
    IndexedTempoChange,
    MeasureChange,
    Bgm
}
