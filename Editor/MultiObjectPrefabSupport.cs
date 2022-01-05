#if NOM_HIERARCHY_PREFAB
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nomnom.HierarchyWindowExtensions.Editor {
	internal static class MultiObjectPrefabSupport {
		private static List<GameObject> _objects;
		private static string _directory;

		[MenuItem("GameObject/Prefab/Multi-Prefab", false, 0)]
		private static void MakePrefab() {
			try {
				if (_objects == null) {
					_objects = new List<GameObject>(Selection.gameObjects);

					string directory = EditorUtility.OpenFolderPanel("Open Folder", "Assets/", string.Empty);
					string assetsPath = Application.dataPath;

					if (assetsPath.Length > directory.Length || string.IsNullOrEmpty(directory)) {
						Debug.LogError("You selected an invalid folder");
					} else {
						_directory = directory;
					}

					// only use objects without parent selectors
					for (int i = 0; i < _objects.Count; i++) {
						GameObject gameObject = _objects[i];
						Transform currentTransform = gameObject.transform;

						for (int j = 0; j < _objects.Count; j++) {
							GameObject checkObj = _objects[j];

							if (checkObj.transform.parent == currentTransform) {
								// nuke this object
								_objects.RemoveAt(j);
								break;
							}
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
			catch (Exception) {
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