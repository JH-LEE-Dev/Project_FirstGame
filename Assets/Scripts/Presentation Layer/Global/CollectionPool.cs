using System.Collections.Generic;

public static class CollectionPool <T> where T : class
{
    private static Stack<Queue<T>> quePool = new Stack<Queue<T>>();
    private static Stack<HashSet<T>> setPool = new Stack<HashSet<T>>();

    public static Queue<T> GetQueue(int _capacity = 0)
    {
        if (0 < quePool.Count)
            return quePool.Pop();
        else
            return new Queue<T>(_capacity);
    }

    public static HashSet<T> GetSet(int _capacity = 0)
    {
        if (0 < setPool.Count)
            return setPool.Pop();
        else
            return new HashSet<T>(_capacity);
    }

    public static void ReturnCollection(Queue<T> _in)
    {
        if (null == _in) 
            return;

        _in.Clear();
        quePool.Push(_in);
    }

    public static void ReturnCollection(HashSet<T> _in)
    {
        if (null == _in)
            return;

        _in.Clear();
        setPool.Push(_in);
    }
}
