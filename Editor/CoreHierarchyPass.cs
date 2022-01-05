using UnityEditor;
using UnityEngine;

namespace Nomnom.HierarchyWindowExtensions.Editor {
	internal static class CoreHierarchyPass {
		[InitializeOnLoadMethod]
		private static void OnLoad() {
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
		}

		private static void HierarchyWindowItemOnGUI(int instanceId, Rect rect) {
			ObjectConnector.Draw(instanceId, rect);
			ObjectIcon.Draw(instanceId, rect);
		}
	}
}