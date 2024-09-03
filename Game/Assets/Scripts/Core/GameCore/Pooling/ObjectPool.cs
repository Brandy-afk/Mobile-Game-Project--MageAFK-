using System.Collections.Generic;

namespace MageAFK.Pooling
{
    public static class ObjectPool<T> where T : new()
{
    private static readonly Stack<T> pool = new();

    public static T Get()
    {
        return (pool.Count > 0) ? pool.Pop() : new T();
    }

    public static void Release(T item)
    {
        pool.Push(item);
    }

    public static void Prewarm(int count)
    {
        for(int i = 0; i < count; i++)
        {
            pool.Push(new T());
        }
    }
}
}