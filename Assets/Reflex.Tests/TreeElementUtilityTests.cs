using System;
using System.Collections.Generic;
using NUnit.Framework;
using Reflex.Editor.DebuggingWindow;

namespace Reflex.Tests
{
    public class TreeElementUtilityTests
    {
        private class TestElement : TreeElement
        {
            public TestElement(string name, int depth)
            {
                Name = name;
                Depth = depth;
            }
        }

        [Test]
        public void TestTreeToListWorks()
        {
            // Arrange
            TestElement root = new TestElement("root", -1);
            root.Children = new List<TreeElement>();
            root.Children.Add(new TestElement("A", 0));
            root.Children.Add(new TestElement("B", 0));
            root.Children.Add(new TestElement("C", 0));

            root.Children[1].Children = new List<TreeElement>();
            root.Children[1].Children.Add(new TestElement("Bchild", 1));

            root.Children[1].Children[0].Children = new List<TreeElement>();
            root.Children[1].Children[0].Children.Add(new TestElement("Bchildchild", 2));

            // Test
            List<TestElement> result = new List<TestElement>();
            TreeElementUtility.TreeToList(root, result);

            // Assert
            string[] namesInCorrectOrder = {"root", "A", "B", "Bchild", "Bchildchild", "C"};
            Assert.AreEqual(namesInCorrectOrder.Length, result.Count, "Result count is not match");
            for (int i = 0; i < namesInCorrectOrder.Length; ++i)
            {
                Assert.AreEqual(namesInCorrectOrder[i], result[i].Name);
            }

            TreeElementUtility.ValidateDepthValues(result);
        }

        [Test]
        public void TestListToTreeWorks()
        {
			// Arrange
			List<TestElement> list = new List<TestElement>();
            list.Add(new TestElement("root", -1));
            list.Add(new TestElement("A", 0));
            list.Add(new TestElement("B", 0));
            list.Add(new TestElement("Bchild", 1));
            list.Add(new TestElement("Bchildchild", 2));
            list.Add(new TestElement("C", 0));

            // Test
            TestElement root = TreeElementUtility.ListToTree(list);

            // Assert
            Assert.AreEqual("root", root.Name);
            Assert.AreEqual(3, root.Children.Count);
            Assert.AreEqual("C", root.Children[2].Name);
            Assert.AreEqual("Bchildchild", root.Children[1].Children[0].Children[0].Name);
        }

        [Test]
        public void TestListToTreeThrowsExceptionIfRootIsInvalidDepth()
        {
			// Arrange
			List<TestElement> list = new List<TestElement>();
            list.Add(new TestElement("root", 0));
            list.Add(new TestElement("A", 1));
            list.Add(new TestElement("B", 1));
            list.Add(new TestElement("Bchild", 2));

            // Test
            bool catchedException = false;
            try
            {
                TreeElementUtility.ListToTree(list);
            }
            catch (Exception)
            {
                catchedException = true;
            }

            // Assert
            Assert.IsTrue(catchedException, "We require the root.depth to be -1, here it is: " + list[0].Depth);
        }
    }
}