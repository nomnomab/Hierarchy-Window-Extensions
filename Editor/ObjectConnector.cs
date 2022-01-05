using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nomnom.HierarchyWindowExtensions.Editor {
	/// <summary>
	/// Adds a custom line type between hierarchy objects
	/// </summary>
	internal static class ObjectConnector {
		private static Texture _dashedLineTexture;
		private static Texture _connectorLineTexture;
		private static Texture _connectorLineEndTexture;
		private static Material _textureMaterial;
		private static Dictionary<GameObject, HierarchyNode> _nodes;
		private static int _rootChildCount;

		[DidReloadScripts]
		private static void ReloadResources() {
			_dashedLineTexture = Resources.Load<Texture>("Dashed_Line");
			_connectorLineTexture = Resources.Load<Texture>("Connector_Line");
			_connectorLineEndTexture = Resources.Load<Texture>("Connector_End_Line");
			_textureMaterial = Resources.Load<Material>("Dashed_Line_Mat");

			UpdateNodes();
		}

		[InitializeOnLoadMethod]
		private static void OnLoad() {
			_nodes = new Dictionary<GameObject, HierarchyNode>();
			
			EditorApplication.hierarchyChanged += OnHierarchyChanged;

			OnHierarchyChanged();
		}

		private static void OnHierarchyChanged() {
			UpdateNodes();

			_rootChildCount = SceneManager.GetActiveScene().GetRootGameObjects().Length;
		}

		public static void Draw(int instanceId, in Rect rect) {
			// draw dashed line when in a parent, but not the root parent
			Object obj = EditorUtility.InstanceIDToObject(instanceId);
			if (!(obj is GameObject gameObject)) {
				return;
			}

			Rect localDashRect = rect;
			localDashRect.x -= 16;
			localDashRect.width = 16;
			localDashRect.height = 16;

			// draw persistent dashed line
			Rect initialDashedRect = localDashRect;
			initialDashedRect.x = 30;

			if (!_nodes.TryGetValue(gameObject, out HierarchyNode node)) {
				return;
			}
			
			Transform transform = node.Transform;
			
			for (int i = 0; i < node.ParentCount; i++) {
				DrawDashedLine(initialDashedRect);

				initialDashedRect.x += 14;
			}

			if (transform.childCount > 0) {
				return;
			}

			bool isEnd = transform.parent && node.Index == transform.parent.childCount - 1 || 
			             !transform.parent && node.Index == _rootChildCount - 1;
			DrawConnectorLine(localDashRect, isEnd);
		}

		private static void DrawDashedLine(in Rect rect) {
			EditorGUI.DrawPreviewTexture(rect, _dashedLineTexture, _textureMaterial);
		}

		private static void DrawConnectorLine(in Rect rect, bool isEnd) {
			EditorGUI.DrawPreviewTexture(rect, isEnd ? _connectorLineEndTexture : _connectorLineTexture, _textureMaterial);
		}

		private static void UpdateNodes() {
			// update tree
			_nodes.Clear();

			GameObject[] objects = Object.FindObjectsOfType<GameObject>();
			foreach (GameObject gameObject in objects) {
				Transform transform = gameObject.transform;
				
				_nodes.Add(gameObject, new HierarchyNode {
					Transform = transform,
					Index = transform.GetSiblingIndex(),
					ParentCount = countParents(transform),
					ChildCount = transform.childCount
				});
			}

			int countParents(Transform transform) {
				int count = 1;
				
				while (transform.parent) {
					transform = transform.parent;
					count++;
				}

				return count;
			}
		}

		private class HierarchyNode {
			public Transform Transform;
			public int Index;
			public int ParentCount;
			public int ChildCount;
		}
	}
}