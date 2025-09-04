using System.Collections.Generic;

namespace Scripts.Unity.Tools
{
    public interface IPoolable
    {
        void Clear();
    }

    public static class ClassPool<T> where T : class, new()
    {
        private static Stack<T> _pool = new();

        public static T Get() => _pool.TryPop(out var value) ? value : new T();
        public static void Release(T value)
        {
            if (value is IPoolable poolable)
                poolable.Clear();
            _pool.Push(value);
        }
    }
}
