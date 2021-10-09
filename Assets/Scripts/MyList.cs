using System.Collections.Generic;

public class MyList<T> : List<T>
{
    public T Back
    {
        get { return this[Count - 1]; }
    }

    public void Remove() => RemoveAt(Count - 1);
}