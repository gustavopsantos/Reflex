using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reflex.Editor.DebuggingWindow
{
	[Serializable]
	public class TreeElement
	{
		[SerializeField] private int _id;
		[SerializeField] private string _name;
		[SerializeField] private int _depth;
		[NonSerialized] private TreeElement _parent;
		[NonSerialized] private List<TreeElement> _children = new List<TreeElement>();

		public int Depth
		{
			get { return _depth; }
			set { _depth = value; }
		}

		public TreeElement Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		public List<TreeElement> Children
		{
			get { return _children; }
			set { _children = value; }
		}

		public bool HasChildren
		{
			get { return Children != null && Children.Count > 0; }
		}

		public string Name
		{
			get { return _name; } set { _name = value; }
		}

		public int Id
		{
			get { return _id; } set { _id = value; }
		}

		public TreeElement ()
		{
		}

		public TreeElement (string name, int depth, int id)
		{
			_name = name;
			_id = id;
			_depth = depth;
		}
	}
}