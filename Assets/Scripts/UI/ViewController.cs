using UnityEngine;

[RequireComponent(typeof(RectTransform))] // Rect Transform必須
public class ViewController : MonoBehaviour
{
    // Rect Transformコンポーネントをキャッシュ
    private RectTransform cachedRectTransform;
    public RectTransform CachedRectTransform
    {
        get
        {
            if (cachedRectTransform == null)
            {
                cachedRectTransform = GetComponent<RectTransform>();
            }
            return cachedRectTransform;
        }
    }

    public virtual string Title
    {
        get { return ""; }
        set { }
    }
}