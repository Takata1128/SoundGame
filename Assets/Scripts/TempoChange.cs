using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempoChange
{
    public float beat; // �e���|�ω����N����beat
    public float tempo; // �e���|�ω����BPM

    // �R���X�g���N�^
    public TempoChange(float beat, float tempo) {
        this.beat = beat;
        this.tempo = tempo;
    }
}
