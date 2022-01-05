using System.Collections.Generic;
using System.Linq;
using Nomnom.EasierCustomPreferences.Editor;
using UnityEditor;
using UnityEngine;

namespace Nomnom.HierarchyWindowExtensions.Editor {
	[PreferencesName("Hierarchy Window Extensions")]
	internal static class PreferencesWindow {
		private const string DEF_LINES = "NOM_HIERARCHY_LINES";
		private const string DEF_ICONS = "NOM_HIERARCHY_ICONS";
		private const string DEF_PREFAB = "NOM_HIERARCHY_PREFAB";
		
		private const string KEY_USE_CUSTOM_LINES = "nomnom.hierarchy-window-extensions.use-lines";
		private const string KEY_USE_CUSTOM_ICONS = "nomnom.hierarchy-window-extensions.use-icons";
		private const string KEY_USE_CUSTOM_PREFAB = "nomnom.hierarchy-window-extensions.use-prefab";
		
		private static GUIContent _useCustomLinesText = new GUIContent("Use Custom Lines");
		private static GUIContent _useCustomIconsText = new GUIContent("Use Custom Icons");
		private static GUIContent _useMultiPrefabText = new GUIContent("Use Multi-Select Prefab Creation");
		
		[SettingsProvider]
		public static SettingsProvider CreateProvider() => CustomPreferences.GetProvider(typeof(PreferencesWindow));
		
		public static Settings OnDeserialize() {
			return new Settings {
				UseLines = EditorPrefs.GetBool(KEY_USE_CUSTOM_LINES, false),
				UseIcons = EditorPrefs.GetBool(KEY_USE_CUSTOM_ICONS, false),
				UseMultiPrefab = EditorPrefs.GetBool(KEY_USE_CUSTOM_PREFAB, false),
			};
		}

		public static void OnSerialize(Settings settings) {
			EditorPrefs.SetBool(KEY_USE_CUSTOM_LINES, settings.UseLines);
			EditorPrefs.SetBool(KEY_USE_CUSTOM_ICONS, settings.UseIcons);
			EditorPrefs.SetBool(KEY_USE_CUSTOM_PREFAB, settings.UseMultiPrefab);
		}
		
		public static void OnGUI(string searchContext, Settings obj) {
			EditorGUI.indentLevel++;
			obj.UseLines = EditorGUILayout.ToggleLeft(_useCustomLinesText, obj.UseLines);
			obj.UseIcons = EditorGUILayout.ToggleLeft(_useCustomIconsText, obj.UseIcons);
			obj.UseMultiPrefab = EditorGUILayout.ToggleLeft(_useMultiPrefabText, obj.UseMultiPrefab);
			EditorGUI.indentLevel--;

			if (GUILayout.Button("Apply")) {
				OnSerialize(obj);
						
				// update preprocessors
				string definesString =
					PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
				List<string> allDefines = definesString.Split(';').ToList();

				bool containsLines = allDefines.Contains(DEF_LINES);
				bool containsIcons = allDefines.Contains(DEF_ICONS);
				bool containsMultiPrefab = allDefines.Contains(DEF_PREFAB);
						
				if (containsLines && !obj.UseLines) {
					allDefines.Remove(DEF_LINES);
				} else if (!containsLines && obj.UseLines) {
					allDefines.Add(DEF_LINES);
				}
						
				if (containsIcons && !obj.UseIcons) {
					allDefines.Remove(DEF_ICONS);
				} else if (!containsIcons && obj.UseIcons) {
					allDefines.Add(DEF_ICONS);
				}
				
				if (containsMultiPrefab && !obj.UseMultiPrefab) {
					allDefines.Remove(DEF_PREFAB);
				} else if (!containsMultiPrefab && obj.UseMultiPrefab) {
					allDefines.Add(DEF_PREFAB);
				}
						
				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines));
				AssetDatabase.Refresh();
			}
		}
		
		internal sealed class Settings {
			public bool UseLines;
			public bool UseIcons;
			public bool UseMultiPrefab;
		}
	}
}