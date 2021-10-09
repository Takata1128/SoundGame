using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteControllerBase : MonoBehaviour
{
    public NoteProperty noteProperty;
    public bool isProcessed = false; // ロングノーツ処理中フラグ

    public virtual void OnKeyDown(JudgementType judgementType) { }
    public virtual void OnKeyUp(JudgementType judgementType) { }
}
