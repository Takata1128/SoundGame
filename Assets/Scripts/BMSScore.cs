using System.Collections.Generic;
using UnityEngine;

public class BMSScore
{
    public int NoteCount { get; set; } = 0;
    public List<BGNote> BGSounds { get; set; }

    public List<BPM> Bpms { get; set; }
    public Lane[] Lanes { get; set; }

    public BMSScore()
    {
        Bpms = new List<BPM>();
        BGSounds = new List<BGNote>();
        Lanes = new Lane[10];
        for (int i = 0; i < 10; i++) Lanes[i] = new Lane();
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

        foreach (BGNote bgNote in BGSounds)
        {
            bgNote.SetSec(Bpms);
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

