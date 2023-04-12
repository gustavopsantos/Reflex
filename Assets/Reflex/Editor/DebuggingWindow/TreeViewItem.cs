using UnityEditor.IMGUI.Controls;

namespace Reflex.Editor.DebuggingWindow
{
    internal class TreeViewItem<T> : TreeViewItem where T : TreeElement
    {
        public T Data { get; }

        public TreeViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName)
        {
            Data = data;
        }
    }
}