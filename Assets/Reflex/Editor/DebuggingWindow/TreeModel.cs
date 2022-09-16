using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Reflex.Editor.DebuggingWindow
{
    // The TreeModel is a utility class working on a list of serializable TreeElements where the order and the depth of each TreeElement define
    // the tree structure. Note that the TreeModel itself is not serializable (in Unity we are currently limited to serializing lists/arrays) but the 
    // input list is.
    // The tree representation (parent and children references) are then build internally using TreeElementUtility.ListToTree (using depth 
    // values of the elements). 
    // The first element of the input list is required to have depth == -1 (the hiddenroot) and the rest to have
    // depth >= 0 (otherwise an exception will be thrown)

    public class TreeModel<T> where T : TreeElement
    {
        public event Action OnModelChanged;

        private IList<T> _data;
        private T _root;
        private int _maxId;

        public T Root
        {
            get { return _root; }
            set { _root = value; }
        }

        public TreeModel(IList<T> data)
        {
            SetData(data);
        }

        private T Find(int id)
        {
            return _data.FirstOrDefault(element => element.Id == id);
        }

        private void SetData(IList<T> data)
        {
            Init(data);
        }

        private void Init(IList<T> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data", "Input data is null. Ensure input is a non-null list.");
            }

            _data = data;
            if (_data.Count > 0)
            {
                _root = TreeElementUtility.ListToTree(data);
            }

            _maxId = _data.Max(e => e.Id);
        }

        private int GenerateUniqueID()
        {
            return ++_maxId;
        }

        public IList<int> GetAncestors(int id)
        {
            var parents = new List<int>();
            TreeElement T = Find(id);
            if (T != null)
            {
                while (T.Parent != null)
                {
                    parents.Add(T.Parent.Id);
                    T = T.Parent;
                }
            }

            return parents;
        }

        public IList<int> GetDescendantsThatHaveChildren(int id)
        {
            T searchFromThis = Find(id);
            if (searchFromThis != null)
            {
                return GetParentsBelowStackBased(searchFromThis);
            }

            return new List<int>();
        }

        private static IList<int> GetParentsBelowStackBased(TreeElement searchFromThis)
        {
            Stack<TreeElement> stack = new Stack<TreeElement>();
            stack.Push(searchFromThis);

            var parentsBelow = new List<int>();
            while (stack.Count > 0)
            {
                TreeElement current = stack.Pop();
                if (current.HasChildren)
                {
                    parentsBelow.Add(current.Id);
                    foreach (var T in current.Children)
                    {
                        stack.Push(T);
                    }
                }
            }

            return parentsBelow;
        }

        public void RemoveElements(IList<int> elementIDs)
        {
            IList<T> elements = _data.Where(element => elementIDs.Contains(element.Id)).ToArray();
            RemoveElements(elements);
        }

        public void RemoveElements(IList<T> elements)
        {
            if (elements.Any(element => element == _root))
            {
                throw new ArgumentException("It is not allowed to remove the root element");
            }

            var commonAncestors = TreeElementUtility.FindCommonAncestorsWithinList(elements);

            foreach (var element in commonAncestors)
            {
                element.Parent.Children.Remove(element);
                element.Parent = null;
            }

            TreeElementUtility.TreeToList(_root, _data);

            Changed();
        }

        public void AddElements(IList<T> elements, TreeElement parent, int insertPosition)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements), "elements is null");
            }

            if (elements.Count == 0)
            {
                throw new ArgumentNullException(nameof(elements), "elements Count is 0: nothing to add");
            }

            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent), "parent is null");
            }

            if (parent.Children == null)
            {
                parent.Children = new List<TreeElement>();
            }

            parent.Children.InsertRange(insertPosition, elements.Cast<TreeElement>());
            foreach (var element in elements)
            {
                element.Parent = parent;
                element.Depth = parent.Depth + 1;
                TreeElementUtility.UpdateDepthValues(element);
            }

            TreeElementUtility.TreeToList(_root, _data);

            Changed();
        }

        public void AddRoot(T root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root), "root is null");
            }

            if (_data == null)
            {
                throw new InvalidOperationException("Internal Error: data list is null");
            }

            if (_data.Count != 0)
            {
                throw new InvalidOperationException("AddRoot is only allowed on empty data list");
            }

            root.Id = GenerateUniqueID();
            root.Depth = -1;
            _data.Add(root);
        }

        public void AddElement(T element, TreeElement parent, int insertPosition)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element), "element is null");
            }

            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent), "parent is null");
            }

            if (parent.Children == null)
            {
                parent.Children = new List<TreeElement>();
            }

            parent.Children.Insert(insertPosition, element);
            element.Parent = parent;

            TreeElementUtility.UpdateDepthValues(parent);
            TreeElementUtility.TreeToList(_root, _data);

            Changed();
        }

        private void Changed()
        {
            OnModelChanged?.Invoke();
        }
    }


    #region Tests

    class TreeModelTests
    {
        [Test]
        public static void TestTreeModelCanAddElements()
        {
            var root = new TreeElement {Name = "Root", Depth = -1};
            var listOfElements = new List<TreeElement>();
            listOfElements.Add(root);

            var model = new TreeModel<TreeElement>(listOfElements);
            model.AddElement(new TreeElement {Name = "Element"}, root, 0);
            model.AddElement(new TreeElement {Name = "Element " + root.Children.Count}, root, 0);
            model.AddElement(new TreeElement {Name = "Element " + root.Children.Count}, root, 0);
            model.AddElement(new TreeElement {Name = "Sub Element"}, root.Children[1], 0);

            // Assert order is correct
            string[] namesInCorrectOrder = {"Root", "Element 2", "Element 1", "Sub Element", "Element"};
            Assert.AreEqual(namesInCorrectOrder.Length, listOfElements.Count, "Result count does not match");
            for (int i = 0; i < namesInCorrectOrder.Length; ++i)
            {
                Assert.AreEqual(namesInCorrectOrder[i], listOfElements[i].Name);
            }

            // Assert depths are valid
            TreeElementUtility.ValidateDepthValues(listOfElements);
        }

        [Test]
        public static void TestTreeModelCanRemoveElements()
        {
            var root = new TreeElement {Name = "Root", Depth = -1};
            var listOfElements = new List<TreeElement>();
            listOfElements.Add(root);

            var model = new TreeModel<TreeElement>(listOfElements);
            model.AddElement(new TreeElement {Name = "Element"}, root, 0);
            model.AddElement(new TreeElement {Name = "Element " + root.Children.Count}, root, 0);
            model.AddElement(new TreeElement {Name = "Element " + root.Children.Count}, root, 0);
            model.AddElement(new TreeElement {Name = "Sub Element"}, root.Children[1], 0);

            model.RemoveElements(new[] {root.Children[1].Children[0], root.Children[1]});

            // Assert order is correct
            string[] namesInCorrectOrder = {"Root", "Element 2", "Element"};
            Assert.AreEqual(namesInCorrectOrder.Length, listOfElements.Count, "Result count does not match");
            for (int i = 0; i < namesInCorrectOrder.Length; ++i)
            {
                Assert.AreEqual(namesInCorrectOrder[i], listOfElements[i].Name);
            }

            // Assert depths are valid
            TreeElementUtility.ValidateDepthValues(listOfElements);
        }
    }

    #endregion
}