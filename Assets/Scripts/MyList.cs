using System.Collections.Generic;

public class MyList<T> : List<T>
{
    public T Back
    {
        get
        {
            if (Count == 0) return default(T);
            return this[Count - 1];
        }
    }

    public void Pop() => RemoveAt(Count - 1);
}