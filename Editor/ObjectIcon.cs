using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Nomnom.HierarchyWindowExtensions.Editor {
	/// <summary>
	/// Replaces the default "cube" icon on GameObjects with their highest order
	/// component.
	/// </summary>
	internal static class ObjectIcon {
		private static Dictionary<GameObject, Texture> _components;

		[InitializeOnLoadMethod]
		private static void OnLoad() {
			_components = new Dictionary<GameObject, Texture>();
			
			EditorApplication.hierarchyChanged += OnHierarchyChanged;

			OnHierarchyChanged();
		}

		private static void OnHierarchyChanged() {
			_components.Clear();
			
			GameObject[] objects = Object.FindObjectsOfType<GameObject>();
			foreach (GameObject gameObject in objects) {
				Component[] components = gameObject.GetComponents<Component>();
				Component toUse = components.Length > 1 ? components[1] : components[0];

				if (toUse is CanvasRenderer && components.Length > 2) {
					toUse = components[2];
				}
				
				Texture newIcon = EditorGUIUtility.ObjectContent(null, toUse.GetType()).image;

				if (newIcon == null) {
					// probably script
					newIcon = EditorGUIUtility.ObjectContent(toUse, toUse.GetType()).image;
				}
				
				_components.Add(gameObject, newIcon);
			}
		}

		public static void Draw(TreeViewItem item, Event e, int instanceId, in Rect rect) {
			Object obj = EditorUtility.InstanceIDToObject(instanceId);
			if (!(obj is GameObject gameObject)) {
				return;
			}

			// get the first component for now
			if (!_components.TryGetValue(gameObject, out Texture texture)) {
				return;
			}

			item.icon = (Texture2D)texture;
		}
	}
}