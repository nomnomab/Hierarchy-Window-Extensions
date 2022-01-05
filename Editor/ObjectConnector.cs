using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Nomnom.HierarchyWindowExtensions.Editor {
	/// <summary>
	/// Adds a custom line type between hierarchy objects
	/// </summary>
	internal static class ObjectConnector {
		private static Texture _dashedLineTexture;
		private static Texture _connectorLineTexture;
		private static Texture _connectorLineEndTexture;
		private static Material _textureMaterial;

		[DidReloadScripts]
		private static void ReloadResources() {
			_dashedLineTexture = Resources.Load<Texture>("Dashed_Line");
			_connectorLineTexture = Resources.Load<Texture>("Connector_Line");
			_connectorLineEndTexture = Resources.Load<Texture>("Connector_End_Line");
			_textureMaterial = Resources.Load<Material>("Dashed_Line_Mat");
		}
		
		public static void Draw(int instanceId, in Rect rect) {
			// draw dashed line when in a parent, but not the root parent
			Object obj = EditorUtility.InstanceIDToObject(instanceId);
			if (!(obj is GameObject gameObject)) {
				return;
			}

			Transform transform = gameObject.transform;
			
			Rect localDashRect = rect;
			localDashRect.x -= 16;
			localDashRect.width = 16;
			localDashRect.height = 16;
			
			Rect initialDashedRect = localDashRect;
			initialDashedRect.x = 32;
			
			// draw persistent dashed line
			DrawDashedLine(initialDashedRect);

			if (transform.childCount > 0) {
				return;
			}

			bool isEnd = transform.parent && transform.GetSiblingIndex() == transform.parent.childCount - 1;
			DrawConnectorLine(localDashRect, isEnd);
		}

		private static void DrawDashedLine(in Rect rect) {
			EditorGUI.DrawPreviewTexture(rect, _dashedLineTexture, _textureMaterial);
		}

		private static void DrawConnectorLine(in Rect rect, bool isEnd) {
			EditorGUI.DrawPreviewTexture(rect, isEnd ? _connectorLineEndTexture : _connectorLineTexture, _textureMaterial);
		}
	}
}