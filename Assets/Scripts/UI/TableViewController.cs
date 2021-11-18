using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class TableViewController<T> : ViewController
{
    protected List<T> tableData = new List<T>();    // リスト項目のデータ
    [SerializeField] private RectOffset padding;    // スクロールさせる内容のパディング
    [SerializeField] private float spacingHeight = 4.0f; // 各セルの間隔

    [SerializeField] private GameObject cellBase; // コピー元のセル
    private LinkedList<TableViewCell<T>> cells = new LinkedList<TableViewCell<T>>(); // セルを保持

    private Rect visibleRect;   // リスト項目をセルとして表示する範囲を示す矩形

    [SerializeField] private RectOffset visibleRectPadding; // visibleRectのパディング

    private Vector2 prevScrollPos; // 前回のスクロール位置を保持


    // Scroll Rect コンポーネントをキャッシュ
    private ScrollRect cachedScrollRect;
    public ScrollRect CachedScrollRect
    {
        get
        {
            if (cachedScrollRect == null)
            {
                cachedScrollRect = GetComponent<ScrollRect>();
            }
            return cachedScrollRect;
        }
    }
    protected void UpdateContentSize()
    {
        float contentHeight = 0.0f;
        for (int i = 0; i < tableData.Count; i++)
        {
            contentHeight += CellHeightAtIndex(i);
            if (i > 0) contentHeight += spacingHeight;
        }
        // スクロールさせる内容の高さを設定
        Vector2 sizeDelta = CachedScrollRect.content.sizeDelta;
        sizeDelta.y = padding.top + contentHeight + padding.bottom;
        CachedScrollRect.content.sizeDelta = sizeDelta;

    }

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        // コピー元のセルを非アクティブ化
        cellBase.SetActive(false);

        // ScrollRectコンポーネントのOnValueChangedイベントのリスナーを設定する
        CachedScrollRect.onValueChanged.AddListener(OnScrollPosChanged);
    }

    // リスト項目に対応するセルの高さを返す
    protected virtual float CellHeightAtIndex(int index)
    {
        return 0.0f;
    }

    private TableViewCell<T> CreateCellForIndex(int index)
    {
        GameObject obj = Instantiate(cellBase) as GameObject;
        obj.SetActive(true);
        TableViewCell<T> cell = obj.GetComponent<TableViewCell<T>>();

        Vector3 scale = cell.transform.localScale;
        Vector2 sizeDelta = cell.CachedRectTransform.sizeDelta;
        Vector2 offsetMin = cell.CachedRectTransform.offsetMin;
        Vector2 offsetMax = cell.CachedRectTransform.offsetMax;

        cell.transform.SetParent(cellBase.transform.parent);

        cell.transform.localScale = scale;
        cell.CachedRectTransform.sizeDelta = sizeDelta;
        cell.CachedRectTransform.offsetMin = offsetMin;
        cell.CachedRectTransform.offsetMax = offsetMax;

        // 指定インデックスのリスト項目に対応するセルとして内容更新
        UpdateCellForIndex(cell, index);
        cells.AddLast(cell);

        return cell;
    }

    // セルの内容更新
    private void UpdateCellForIndex(TableViewCell<T> cell, int index)
    {
        cell.DataIndex = index;

        if (cell.DataIndex >= 0 && cell.DataIndex <= tableData.Count - 1)
        {
            // セルに対応するリスト項目が存在
            cell.gameObject.SetActive(true);
            cell.UpdateContent(tableData[cell.DataIndex]);
            cell.Height = CellHeightAtIndex(cell.DataIndex);
        }
        else
        {
            // セルに対応するリスト項目がない
            cell.gameObject.SetActive(false);
        }
    }

    // visibleRectを更新するためのメソッド
    private void UpdateVisibleRect()
    {
        // visibleRectの位置はスクロールさせる内容の基準点からの相対位置
        visibleRect.x = CachedScrollRect.content.anchoredPosition.x + visibleRectPadding.left;
        visibleRect.y = -CachedScrollRect.content.anchoredPosition.y + visibleRectPadding.top;

        // visibleRectのサイズはスクロールビューのサイズ＋パディング
        visibleRect.width = CachedRectTransform.rect.width + visibleRectPadding.left + visibleRectPadding.right;
        visibleRect.height = CachedRectTransform.rect.height + visibleRectPadding.top + visibleRectPadding.bottom;
    }

    // テーブルビューの表示内容更新
    protected void UpdateContents()
    {
        UpdateContentSize();
        UpdateVisibleRect();

        // セルが一つもないとき
        // visibleRectの範囲に入る最初のリスト項目を探し、それに対応するセルを作成する
        if (cells.Count < 1)
        {
            Vector2 cellTop = new Vector2(0.0f, -padding.top);
            for (int i = 0; i < tableData.Count; i++)
            {
                float cellHeight = CellHeightAtIndex(i);
                Vector2 cellBottom = cellTop + new Vector2(0.0f, -cellHeight);
                if ((cellTop.y <= visibleRect.y && cellTop.y >= visibleRect.y - visibleRect.height) ||
                    (cellBottom.y <= visibleRect.y && cellBottom.y >= visibleRect.y - visibleRect.height))
                {
                    TableViewCell<T> cell = CreateCellForIndex(i);
                    cell.Top = cellTop;
                    break;
                }
                cellTop = cellBottom + new Vector2(0.0f, spacingHeight);
            }
            FillVisibleRectWithCells();
        }
        else
        {
            LinkedListNode<TableViewCell<T>> node = cells.First;
            UpdateCellForIndex(node.Value, node.Value.DataIndex);
            node = node.Next;
            while (node != null)
            {
                UpdateCellForIndex(node.Value, node.Previous.Value.DataIndex + 1);
                node.Value.Top = node.Previous.Value.Bottom + new Vector2(0.0f, -spacingHeight);
                node = node.Next;
            }
            FillVisibleRectWithCells();
        }
    }

    //  visibleRectの範囲内に表示される分のセルを作成するメソッド
    private void FillVisibleRectWithCells()
    {
        if (cells.Count < 1) return;

        // 表示されている最後のセルに対応するリスト項目の次があって
        // そのセルがvisibleRectの範囲内
        TableViewCell<T> lastCell = cells.Last.Value;
        int nextCellDataIndex = lastCell.DataIndex + 1;
        Vector2 nextCellTop = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);
        while (nextCellDataIndex < tableData.Count && nextCellTop.y >= visibleRect.y - visibleRect.height)
        {
            TableViewCell<T> cell = CreateCellForIndex(nextCellDataIndex);
            cell.Top = nextCellTop;

            lastCell = cell;
            nextCellDataIndex = lastCell.DataIndex + 1;
            nextCellTop = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);
        }
    }

    // スクロールビューがスクロールされたときに呼ばれる
    public void OnScrollPosChanged(Vector2 ScrollPos)
    {
        // VisibleRectを更新する
        UpdateVisibleRect();
        // スクロールした方向によってセルを再利用して表示を更新する
        ReuseCells((ScrollPos.y < prevScrollPos.y) ? 1 : -1);
        prevScrollPos = ScrollPos;
    }

    private void ReuseCells(int scrollDirection)
    {
        if (cells.Count < 1)
        {
            return;
        }

        if (scrollDirection > 0)
        {
            // 上にスクロールしている場合、visibleRectの範囲より上にあるセルを
            // 順に下に移動して内容を更新する
            TableViewCell<T> firstCell = cells.First.Value;
            while (firstCell.Bottom.y > visibleRect.y)
            {
                TableViewCell<T> lastCell = cells.Last.Value;
                UpdateCellForIndex(firstCell, lastCell.DataIndex + 1);
                firstCell.Top = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);

                cells.AddLast(firstCell);
                cells.RemoveFirst();
                firstCell = cells.First.Value;
            }
            // visibleRectの範囲内に空きがあればセルを作成する
            FillVisibleRectWithCells();
        }
        else if (scrollDirection < 0)
        {
            // 下にスクロールしている場合、visibleRectの範囲より下にあるセルを
            // 順に上に移動して内容を更新
            TableViewCell<T> lastCell = cells.Last.Value;
            while (lastCell.Top.y < visibleRect.y - visibleRect.height)
            {
                TableViewCell<T> firstCell = cells.First.Value;
                UpdateCellForIndex(lastCell, firstCell.DataIndex - 1);
                lastCell.Bottom = firstCell.Top + new Vector2(0.0f, spacingHeight);
                cells.AddFirst(lastCell);
                cells.RemoveLast();
                lastCell = cells.Last.Value;
            }
        }
    }
}
