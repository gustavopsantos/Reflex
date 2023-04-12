using System;
using System.Collections.Generic;

namespace Reflex.Editor.DebuggingWindow
{
	public static class TreeElementUtility
	{
		public static void TreeToList<T>(T root, IList<T> result) where T : TreeElement
		{
			if (result == null)
				throw new NullReferenceException("The input 'IList<T> result' list is null");
			result.Clear();

			Stack<T> stack = new Stack<T>();
			stack.Push(root);

			while (stack.Count > 0)
			{
				T current = stack.Pop();
				result.Add(current);

				if (current.Children != null && current.Children.Count > 0)
				{
					for (int i = current.Children.Count - 1; i >= 0; i--)
					{
						stack.Push((T)current.Children[i]);
					}
				}
			}
		}

		// Returns the root of the tree parsed from the list (always the first element).
		// Important: the first item and is required to have a depth value of -1. 
		// The rest of the items should have depth >= 0. 
		public static T ListToTree<T>(IList<T> list) where T : TreeElement
		{
			// Validate input
			ValidateDepthValues (list);

			// Clear old states
			foreach (var element in list)
			{
				element.Parent = null;
				element.Children = null;
			}

			// Set child and parent references using depth info
			for (int parentIndex = 0; parentIndex < list.Count; parentIndex++)
			{
				var parent = list[parentIndex];
				bool alreadyHasValidChildren = parent.Children != null;
				if (alreadyHasValidChildren)
					continue;

				int parentDepth = parent.Depth;
				int childCount = 0;

				// Count children based depth value, we are looking at children until it's the same depth as this object
				for (int i = parentIndex + 1; i < list.Count; i++)
				{
					if (list[i].Depth == parentDepth + 1)
						childCount++;
					if (list[i].Depth <= parentDepth)
						break;
				}

				// Fill child array
				List<TreeElement> childList = null;
				if (childCount != 0)
				{
					childList = new List<TreeElement>(childCount); // Allocate once
					childCount = 0;
					for (int i = parentIndex + 1; i < list.Count; i++)
					{
						if (list[i].Depth == parentDepth + 1)
						{
							list[i].Parent = parent;
							childList.Add(list[i]);
							childCount++;
						}

						if (list[i].Depth <= parentDepth)
							break;
					}
				}

				parent.Children = childList;
			}

			return list[0];
		}

		// Check state of input list
		public static void ValidateDepthValues<T>(IList<T> list) where T : TreeElement
		{
			if (list.Count == 0)
				throw new ArgumentException("list should have items, count is 0, check before calling ValidateDepthValues", "list");

			if (list[0].Depth != -1)
				throw new ArgumentException("list item at index 0 should have a depth of -1 (since this should be the hidden root of the tree). Depth is: " + list[0].Depth, "list");

			for (int i = 0; i < list.Count - 1; i++)
			{
				int depth = list[i].Depth;
				int nextDepth = list[i + 1].Depth;
				if (nextDepth > depth && nextDepth - depth > 1)
					throw new ArgumentException(string.Format("Invalid depth info in input list. Depth cannot increase more than 1 per row. Index {0} has depth {1} while index {2} has depth {3}", i, depth, i + 1, nextDepth));
			}

			for (int i = 1; i < list.Count; ++i)
				if (list[i].Depth < 0)
					throw new ArgumentException("Invalid depth value for item at index " + i + ". Only the first item (the root) should have depth below 0.");

			if (list.Count > 1 && list[1].Depth != 0)
				throw new ArgumentException("Input list item at index 1 is assumed to have a depth of 0", "list");
		}
	}
}