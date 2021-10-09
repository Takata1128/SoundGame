using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteProperty
{
    public float beatBegin;
    public float beatEnd;
    public float secBegin;
    public float secEnd;

    public int lane;
    public NoteType noteType;

    public NoteProperty(float beatBegin, float beatEnd, int lane, NoteType noteType)
    {
        this.beatBegin = beatBegin;
        this.beatEnd = beatEnd;
        this.lane = lane;
        this.noteType = noteType;
    }
}

public enum NoteType
{
    Single,
    Long,
    BGSound
}

