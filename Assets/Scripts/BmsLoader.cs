using System.Collections;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class BmsLoader
{
    private static string MainDataPattern = @"#([0-9]{3})([0-9A-Z]{2}):(.*)";

    private static List<string> HeaderPatterns = new List<string> {
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

    private static string SoundPattern =
        @"#WAV([0-9A-Z]{2}) (.*)"; // #WAV00~ZZ

    private static Dictionary<char, int> LanePairs = new Dictionary<char, int>(){
        {'1',1},
        {'2',2},
        {'3',3},
        {'4',4},
        {'5',5},
        {'6',8},
        {'8',6},
        {'9',7},
    };

    public BMSHeader BmsHeader = new BMSHeader();

    public BMSScore BmsScore = new BMSScore();


    // �w�b�_�[���A�f�[�^
    public Dictionary<string, string> headerData = new Dictionary<string, string>() {
        { "PLAYER","NULL"},
        { "TITLE","NULL"},
        { "ARTIST","NULL"},
        { "PLAYLEVEL","NULL"},
        { "RANK","NULL"},
        { "BPM","0"}
    };

    public Dictionary<int, string> SoundPathes = new Dictionary<int, string>();


    // �e�����̒���(beat�P��,4��4����4���q)
    public float[] measureLength = Enumerable.Repeat(4f, 1000).ToArray();

    // �e���[���ōŌ��LN��ON�ɂȂ���beat
    private float[] longNoteBeginBuffers = new float[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
    private int[] longNoteSoundBuffers = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };



    // (�R���X�g���N�^) BMS�t�@�C����ǂݍ���
    public BmsLoader(string filePath)
    {
        BmsHeader.Path = filePath;
        BmsHeader.ParentPath = System.IO.Path.GetDirectoryName(filePath);

        // BMS�t�@�C����ǂݍ��݁A�e�s��ێ�
        var lines = File.ReadAllLines(filePath, Encoding.UTF8);

        foreach (var line in lines)
        {
            LoadHeaderLine(line);
        }

        SetBmsHeader();
        BmsHeader.SoundPathes = SoundPathes;


        BmsScore.AddBPM(
            new BPM(Convert.ToSingle(headerData["BPM"]), 0f)
        );

        foreach (var line in lines)
        {
            LoadMainDataLine(line);
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

    }

    // �w�b�_�[�s�̂ݓǂݍ���
    private void LoadHeaderLine(string line)
    {
        Match wavMatch = Regex.Match(line, SoundPattern);

        if (wavMatch.Success)
        {
            var index = Util.Decode(wavMatch.Groups[1].Value);
            var path = wavMatch.Groups[2].Value;
            SoundPathes[index] = path;
        }
        else
        {
            foreach (var headerPattern in HeaderPatterns)
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

    private void SetBmsHeader()
    {
        // Initialize BmsHeader
        if (headerData.ContainsKey("PLAYER"))
        {
            BmsHeader.Player = Util.Decode(headerData["PLAYER"]);
        }
        if (headerData.ContainsKey("ARTIST"))
        {
            BmsHeader.Artist = headerData["ARTIST"];
        }
        if (headerData.ContainsKey("LEVEL"))
        {
            BmsHeader.Level = Util.Decode(headerData["LEVEL"]);
        }
        if (headerData.ContainsKey("GENRE"))
        {
            BmsHeader.Genre = headerData["GENRE"];
        }
        if (headerData.ContainsKey("RANK"))
        {
            BmsHeader.Rank = Util.Decode(headerData["RANK"]);
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
    private void LoadMainDataLine(string line)
    {
        var match = Regex.Match(line, MainDataPattern);

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
                        if (!LanePairs.ContainsKey(channel[1]))
                        {
                            Debug.Log($"Invalid LaneChannel: {BmsHeader.Title}, channel={channel[1]}");
                            continue;
                        }
                        int lane = LanePairs[channel[1]];

                        switch (dataType)
                        {
                            case DataType.SingleNote: // �V���O���m�[�c
                                BmsScore.AddNote(lane, beat, Util.Decode(objNum));
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
                                    BmsScore.AddLongNote(lane, longNoteBeginBuffers[lane], beat, longNoteSoundBuffers[lane], Util.Decode(objNum));
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
                        BmsScore.AddBPM(beat, bpm);
                    }
                    // �C���f�b�N�X�w��^�C�v�̃e���|�ω�
                    else if (dataType == DataType.IndexedTempoChange)
                    {
                        // headerData��"BPMxx"�Ƃ����L�[�����l�������ɕϊ�
                        float bpm = Convert.ToSingle(headerData["BPM" + objNum]);
                        BmsScore.AddBPM(beat, bpm);
                    }
                    // BGM
                    else if (dataType == DataType.Bgm)
                    {
                        string index = objNum;
                        // bgmProperties.Add(new BgmProperty(beat, index));
                        BmsScore.AddBGSound(beat, Util.Decode(objNum));
                    }
                }
            }
        }
    }

    // �`�����l���ԍ�����f�[�^�̎�ނ����߂�
    private DataType GetDataType(string channel)
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
