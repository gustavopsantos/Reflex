using System.Collections.Generic;

namespace Reflex.Scripts.Core
{
    public class Node<T>
    {
        public T Value { get; }
        public T Parent { get; private set; }
        public List<Node<T>> Children = new List<Node<T>>();

        public Node(T value)
        {
            Value = value;
        }
    }
}