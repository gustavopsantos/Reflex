using System.Collections.Generic;

namespace Reflex.Scripts.Utilities
{
    public static class QueueExtensions
    {
        public static Queue<T> With<T>(this Queue<T> queue, T item)
        {
            queue.Enqueue(item);
            return queue;
        }
        
        public static bool TryDequeue<T>(this Queue<T> source, out T item)
        {
            if (source.Count > 0)
            {
                item = source.Dequeue();
                return true;
            }

            item = default;
            return false;
        }
    }
}