using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Nomnom.HierarchyWindowExtensions.Editor {
	/// <summary>
	/// Replaces the default "cube" icon on GameObjects with their highest order
	/// component.
	/// </summary>
	internal static class ObjectIcon {
		private static Material _textureMaterial;
		private static Color32 _proColor = new Color32(56, 56, 56, 255);
		private static Color32 _nonProColor = new Color32(201, 201, 201, 255);
		
		[DidReloadScripts]
		private static void ReloadResources() {
			_textureMaterial = Resources.Load<Material>("Icon_Mat");
		}
		
		public static void Draw(int instanceId, in Rect rect) {
			Object obj = EditorUtility.InstanceIDToObject(instanceId);
			if (!(obj is GameObject gameObject)) {
				return;
			}

			// get the first component for now
			Component component = gameObject.GetComponent<Component>();
			Texture newIcon = EditorGUIUtility.ObjectContent(null, component.GetType()).image;
			Rect iconRect = rect;
			iconRect.width = iconRect.height;

			Color color = EditorGUIUtility.isProSkin ? _proColor : _nonProColor;
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
			EditorGUI.DrawRect(iconRect, color);
			EditorGUI.DrawPreviewTexture(iconRect, newIcon, _textureMaterial);
		}
	}
}