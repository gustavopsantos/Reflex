using System.Collections.Generic;

namespace Reflex.Scripts.Extensions
{
    internal static class StackExtensions
    {
        internal static T PushNew<T>(this Stack<T> stack) where T : new()
        {
            var instance = new T();
            stack.Push(instance);
            return instance;
        }
    }
}