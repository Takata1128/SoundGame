using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableViewCell<T> : ViewController
{
    public virtual void UpdateContent(T itemData)
    {

    }

    // セルに対応するリスト項目のインデックス
    public int DataIndex { get; set; }

    // セルの高さを取得、設定
    public float Height
    {
        get { return CachedRectTransform.sizeDelta.y; }
        set
        {
            Vector2 sizeDelta = CachedRectTransform.sizeDelta;
            sizeDelta.y = value;
            CachedRectTransform.sizeDelta = sizeDelta;
        }
    }

    // セルの上端の位置を取得、設定
    public Vector2 Top
    {
        get
        {
            Vector3[] corners = new Vector3[4];
            CachedRectTransform.GetLocalCorners(corners);
            return CachedRectTransform.anchoredPosition + new Vector2(0.0f, corners[1].y);
        }
        set
        {
            Vector3[] corners = new Vector3[4];
            CachedRectTransform.GetLocalCorners(corners);
            CachedRectTransform.anchoredPosition = value - new Vector2(0.0f, corners[1].y);
        }
    }

    // セルの下端の位置を取得、設定
    public Vector2 Bottom
    {
        get
        {
            Vector3[] corners = new Vector3[4];
            CachedRectTransform.GetLocalCorners(corners);
            return CachedRectTransform.anchoredPosition + new Vector2(0.0f, corners[3].y);
        }
        set
        {
            Vector3[] corners = new Vector3[4];
            CachedRectTransform.GetLocalCorners(corners);
            CachedRectTransform.anchoredPosition = value - new Vector2(0.0f, corners[3].y);
        }
    }

}
