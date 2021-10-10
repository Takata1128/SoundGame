using System.Collections.Generic;

public class Lane
{
    public List<Note> NoteList;
    public Lane()
    {
        NoteList = new List<Note>();
    }
}

public abstract class BMSObject : System.IComparable<BMSObject>
{
    public float BeatBegin;

    public float SecBegin;



    public BMSObject(float beatBegin)
    {
        BeatBegin = beatBegin;
    }

    public int CompareTo(BMSObject other)
    {
        if (BeatBegin < other.BeatBegin) return 1;
        if (BeatBegin == other.BeatBegin) return 0;
        return -1;
    }
}

public class Note : BMSObject
{
    public int KeySound { get; private set; }
    public float BeatEnd;
    public float SecEnd;

    public Note(int keySound, float beatBegin) : base(beatBegin)
    {
        KeySound = keySound;
    }
}

public class LongNote : Note
{
    public new void SetSec(List<BPM> bpms)
    {
        SecBegin = Util.ToSec(BeatBegin, bpms);
        SecEnd = Util.ToSec(BeatEnd, bpms);
    }

    public int KeySoundEnd { get; private set; }
    public LongNote(int keySound, int keySoundEnd, float beatBegin, float beatEnd) : base(keySound, beatBegin)
    {
        BeatEnd = beatEnd;
        KeySoundEnd = keySoundEnd;
    }
}

public class BGNote : Note
{
    public BGNote(int keySound, float beatBegin) : base(keySound, beatBegin)
    {

    }
}

public class BPM : BMSObject
{
    public float Bpm { get; private set; }
    public BPM(float bpm, float beatBegin) : base(beatBegin)
    {
        Bpm = bpm;
    }

    public BPM(BPM bpm) : base(bpm.BeatBegin)
    {
        Bpm = bpm.Bpm;
    }
}