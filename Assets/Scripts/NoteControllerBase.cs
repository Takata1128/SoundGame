using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteControllerBase : MonoBehaviour
{
    public int Lane { get; set; }
    public Note Note { get; set; }
    public bool IsProcessed { get; set; } = false; // �����O�m�[�c�������t���O

    protected bool renderFlag = false;

    // Update is called once per frame
    [SerializeField] protected GameObject prefabBurst;
    [SerializeField] protected float deleteTime;
    [SerializeField] protected float burstSize;

    public virtual bool CheckMiss() { return false; }

    public virtual void OnKeyDown(JudgementType judgementType) { }
    public virtual void OnKeyUp(JudgementType judgementType) { }
}
