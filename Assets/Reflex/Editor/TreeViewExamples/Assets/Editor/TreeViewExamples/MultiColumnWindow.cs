using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor.TreeViewExamples
{
	class MultiColumnWindow : EditorWindow
	{
		[NonSerialized] bool m_Initialized;
		[SerializeField] TreeViewState m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading
		[SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
		SearchField m_SearchField;
		MultiColumnTreeView m_TreeView;

		[MenuItem("Reflex/Debugger")]
		public static MultiColumnWindow GetWindow ()
		{
			var window = GetWindow<MultiColumnWindow>();
			window.titleContent = new GUIContent("Reflex Debugger");
			window.Focus();
			window.Repaint();
			return window;
		}

		Rect multiColumnTreeViewRect
		{
			get { return new Rect(20, 30, position.width-40, position.height-50); }
		}

		Rect toolbarRect
		{
			get { return new Rect (20f, 10f, position.width-40f, 20f); }
		}

		public MultiColumnTreeView treeView
		{
			get { return m_TreeView; }
		}

		void InitIfNeeded ()
		{
			if (!m_Initialized)
			{
				// Check if it already exists (deserialized from window layout file or scriptable object)
				if (m_TreeViewState == null)
					m_TreeViewState = new TreeViewState();

				bool firstInit = m_MultiColumnHeaderState == null;
				var headerState = MultiColumnTreeView.CreateDefaultMultiColumnHeaderState();
				if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
					MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
				m_MultiColumnHeaderState = headerState;
				
				var multiColumnHeader = new MyMultiColumnHeader(headerState);
				if (firstInit)
					multiColumnHeader.ResizeToFit ();

				var treeModel = new TreeModel<MyTreeElement>(GetData());
				
				m_TreeView = new MultiColumnTreeView(m_TreeViewState, multiColumnHeader, treeModel);

				m_SearchField = new SearchField();
				m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;

				m_Initialized = true;
			}
		}

		// private const string ContainerIcon = "d_PrefabModel On Icon";
		private const string ContainerIcon = "PreMatCube";
		private const string ObjectIcon = "curvekeyframeselected";
		private const string ContractIcon = "P4_Updating";
		
		IList<MyTreeElement> GetData ()
		{
			var root = new MyTreeElement("Root", -1, 0, ContainerIcon, "-");
			
			var projectContainer = new MyTreeElement("ProjectContainer", 0, 1, ContainerIcon, "C");
			projectContainer.parent = root;
			root.children.Add(projectContainer);
			
			var projectContainer2 = new MyTreeElement("ProjectContainer2", 0, 5, ContainerIcon, "C");
			projectContainer2.parent = root;
			root.children.Add(projectContainer2);
			
			var sceneContainer = new MyTreeElement("SceneContainer", 1, 2, ContainerIcon, "C");
			sceneContainer.parent = projectContainer;
			projectContainer.children.Add(sceneContainer);
			
			var scopedContainer = new MyTreeElement("ScopedContainer", 2, 6, ContainerIcon, "C");
			scopedContainer.parent = sceneContainer;
			sceneContainer.children.Add(scopedContainer);
			
			var projectContainerResolver = new MyTreeElement("SingletonResolver<ITimeSource>", 1, 3, ContractIcon, "R");
			projectContainerResolver.parent = projectContainer;
			projectContainer.children.Add(projectContainerResolver);
			
			var timeSourceInstance = new MyTreeElement("LocalTimeSource", 2, 4, ObjectIcon, "O");
			timeSourceInstance.parent = projectContainerResolver;
			projectContainerResolver.children.Add(timeSourceInstance);

			var list = new List<MyTreeElement>()
			{
				root,
				projectContainer,
				sceneContainer,
				scopedContainer,
				projectContainerResolver,
				timeSourceInstance,
				projectContainer2,
			};

			return list;
		}

		private void OnGUI ()
		{
			InitIfNeeded();
			SearchBar (toolbarRect);
			DoTreeView (multiColumnTreeViewRect);
		}

		private void SearchBar (Rect rect)
		{
			treeView.searchString = m_SearchField.OnGUI (rect, treeView.searchString);
		}

		private void DoTreeView (Rect rect)
		{
			m_TreeView.OnGUI(rect);
		}
	}

	internal class MyMultiColumnHeader : MultiColumnHeader
	{
		Mode m_Mode;

		public enum Mode
		{
			LargeHeader,
			DefaultHeader,
			MinimumHeaderWithoutSorting
		}

		public MyMultiColumnHeader(MultiColumnHeaderState state)
			: base(state)
		{
			mode = Mode.DefaultHeader;
			height = MultiColumnHeader.DefaultGUI.minimumHeight;
		}

		public Mode mode
		{
			get
			{
				return m_Mode;
			}
			set
			{
				m_Mode = value;
				switch (m_Mode)
				{
					case Mode.LargeHeader:
						canSort = true;
						height = 37f;
						break;
					case Mode.DefaultHeader:
						canSort = true;
						height = DefaultGUI.defaultHeight;
						break;
					case Mode.MinimumHeaderWithoutSorting:
						canSort = false;
						height = DefaultGUI.minimumHeight;
						break;
				}
			}
		}

		protected override void ColumnHeaderGUI (MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
		{
			// Default column header gui
			base.ColumnHeaderGUI(column, headerRect, columnIndex);

			// Add additional info for large header
			if (mode == Mode.LargeHeader)
			{
				// Show example overlay stuff on some of the columns
				if (columnIndex > 2)
				{
					headerRect.xMax -= 3f;
					var oldAlignment = EditorStyles.largeLabel.alignment;
					EditorStyles.largeLabel.alignment = TextAnchor.UpperRight;
					GUI.Label(headerRect, 36 + columnIndex + "%", EditorStyles.largeLabel);
					EditorStyles.largeLabel.alignment = oldAlignment;
				}
			}
		}
	}
}