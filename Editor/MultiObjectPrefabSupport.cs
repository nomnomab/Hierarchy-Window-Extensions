#if NOM_HIERARCHY_PREFAB
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nomnom.HierarchyWindowExtensions.Editor {
	internal static class MultiObjectPrefabSupport {
		private static List<GameObject> _objects;
		private static string _directory;

		[MenuItem("GameObject/Prefab/Multi-Prefab", false, 0)]
		private static void MakePrefab() {
			if (_objects == null) {
				_objects = new List<GameObject>();

				GameObject[] tmpObjects = Selection.gameObjects;

				string directory = EditorUtility.OpenFolderPanel("Open Folder", "Assets/", string.Empty);
				string assetsPath = Application.dataPath;

				if (assetsPath.Length > directory.Length || string.IsNullOrEmpty(directory)) {
					Debug.LogError("You selected an invalid folder");
				} else {
					_directory = directory;
				}

				// only use objects without parent selectors
				for (int i = 0; i < tmpObjects.Length; i++) {
					GameObject gameObject = tmpObjects[i];
					Transform currentTransform = gameObject.transform;
					bool isValid = true;

					Transform parent = currentTransform.parent;
					while (parent) {
						if (tmpObjects.Contains(parent.gameObject)) {
							isValid = false;
						}

						parent = parent.parent;
					}

					if (isValid) {
						_objects.Add(gameObject);
					}
				}
			}

			GameObject obj = _objects[0];

			if (!string.IsNullOrEmpty(_directory)) {
				PrefabUtility.SaveAsPrefabAssetAndConnect(obj, $"{_directory}/{obj.name}.prefab", InteractionMode.UserAction);
			}

			_objects.RemoveAt(0);

			if (_objects.Count == 0) {
				_objects = null;
				_directory = null;
			}
		}
		
		[MenuItem("GameObject/Prefab/Multi-Prefab", true)]
		private static bool DoThingValidate() {
			return Selection.gameObjects != null && Selection.gameObjects.Length > 1;
		}
	}
}
#endif