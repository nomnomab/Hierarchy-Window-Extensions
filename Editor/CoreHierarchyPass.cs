using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nomnom.HierarchyWindowExtensions.Editor {
	internal static class CoreHierarchyPass {
		private static readonly Type _sceneHierarchyWindowType = Type.GetType(
				"UnityEditor.SceneHierarchyWindow, UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
		private static readonly PropertyInfo _lastInteractedHierarchyWindow = _sceneHierarchyWindowType
			.GetProperty("lastInteractedHierarchyWindow", BindingFlags.Public | BindingFlags.Static);

		private static object _treeView;
		private static MethodInfo _findItem;
		private static MethodInfo _initTree;
		private static Object _lastSelection;

		[InitializeOnLoadMethod]
		private static void OnLoad() {
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
		}

		private static void HierarchyWindowItemOnGUI(int instanceId, Rect rect) {
			if (_treeView == null) {
				AssignTreeView();
			}

			if (PreferencesWindow.UseCustomLines) {
				ObjectConnector.Draw(instanceId, rect);
			}

			if (PreferencesWindow.UseCustomIcons) {
				Event e = Event.current;
				TreeViewItem item = GetItem(instanceId);

				ObjectIcon.Draw(item, e, instanceId, rect);
			}
		}

		public static void ResetTree() {
			object lastWindow = _lastInteractedHierarchyWindow.GetValue(null);
			object sceneHierarchy = lastWindow
				.GetType()
				.GetField("m_SceneHierarchy", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(lastWindow);
			_initTree.Invoke(sceneHierarchy, null);
			
			EditorApplication.RepaintHierarchyWindow();
		}

		private static TreeViewItem GetItem(int id) {
			return (TreeViewItem)_findItem?.Invoke(_treeView, new object[] { id });
		}
		
		private static void AssignTreeView() {
			object lastWindow = _lastInteractedHierarchyWindow.GetValue(null);
			object sceneHierarchy = lastWindow
				.GetType()
				.GetField("m_SceneHierarchy", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(lastWindow);
			_treeView = sceneHierarchy.GetType()
				.GetField("m_TreeView", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(sceneHierarchy);
			_findItem = _treeView.GetType().GetMethod("FindItem", BindingFlags.Public | BindingFlags.Instance);
			_initTree = sceneHierarchy.GetType()
				.GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance);
		}
	}
}