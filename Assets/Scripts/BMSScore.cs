using System.Collections.Generic;
using UnityEngine;

public class BMSScore
{
    public int NoteCount { get; set; } = 0;
    public MyList<Note> BGSounds { get; set; }

    public MyList<BPM> Bpms { get; set; }
    public Lane[] Lanes { get; set; }

    public BMSScore()
    {
        Bpms = new MyList<BPM>();
        BGSounds = new MyList<Note>();
        Lanes = new Lane[9];
        for (int i = 0; i < 9; i++) Lanes[i] = new Lane();
    }

    public void SetNotesSec()
    {
        for (int i = 0; i < Lanes.Length; i++)
        {
            foreach (Note note in Lanes[i].NoteList)
            {
                note.SetSec(Bpms);
            }
        }
    }

    public void AddNote(int lane, float beatBegin, int keySound)
    {
        Lanes[lane].NoteList.Add(new Note(keySound, beatBegin));
        ++NoteCount;
    }

    public void AddLongNote(int lane, float beatBegin, float beatEnd, int keySoundBegin, int keySoundEnd)
    {
        Lanes[lane].NoteList.Add(new LongNote(keySoundBegin, keySoundEnd, beatBegin, beatEnd));
        NoteCount += 2;
    }

    public void AddBGSound(float beat, int keySound)
    => BGSounds.Add(new BGNote(keySound, beat));

    public void AddBPM(float beat, float bpm)
        => Bpms.Add(new BPM(bpm, beat));

    public void AddBPM(BPM bpm)
        => Bpms.Add(bpm);

}

