using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteControllerBase : MonoBehaviour
{
    public int lane;
    public Note note;
    public bool isProcessed = false; // �����O�m�[�c�������t���O

    // Update is called once per frame
    [SerializeField] protected GameObject prefabBurst;
    [SerializeField] protected float deleteTime;
    [SerializeField] protected float burstSize;

    public virtual bool CheckMiss() { return false; }

    public virtual void OnKeyDown(JudgementType judgementType) { }
    public virtual void OnKeyUp(JudgementType judgementType) { }
}
