using System.Collections.Generic;

namespace Reflex
{
    public class Node<T>
    {
        public Node<T> Parent { get; private set; }
        public List<Node<T>> Children { get; private set; }
    }
}