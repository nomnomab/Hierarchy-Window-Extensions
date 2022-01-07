using System.Collections.Generic;
using System.Linq;
using Nomnom.EasierCustomPreferences.Editor;
using UnityEditor;
using UnityEngine;

namespace Nomnom.HierarchyWindowExtensions.Editor {
	[PreferencesName("Hierarchy Window Extensions", "Nomnom")]
	internal static class PreferencesWindow {
		public static bool UseCustomLines { get; private set; }
		public static bool UseCustomIcons { get; private set; }
		
		private const string DEF_PREFAB = "NOM_HIERARCHY_PREFAB";
		
		private const string KEY_USE_CUSTOM_LINES = "nomnom.hierarchy-window-extensions.use-lines";
		private const string KEY_USE_CUSTOM_ICONS = "nomnom.hierarchy-window-extensions.use-icons";
		private const string KEY_USE_CUSTOM_PREFAB = "nomnom.hierarchy-window-extensions.use-prefab";
		
		private static GUIContent _useCustomLinesText = new GUIContent("Use Custom Lines");
		private static GUIContent _useCustomIconsText = new GUIContent("Use Custom Icons");
		private static GUIContent _useMultiPrefabText = new GUIContent("Use Multi-Select Prefab Creation");
		
		[InitializeOnLoadMethod]
		private static void AssignFromPrefs() {
			UseCustomLines = EditorPrefs.GetBool(KEY_USE_CUSTOM_LINES, true);
			UseCustomIcons = EditorPrefs.GetBool(KEY_USE_CUSTOM_ICONS, true);
		}
		
		[SettingsProvider]
		public static SettingsProvider CreateProvider() => CustomPreferences.GetProvider(typeof(PreferencesWindow));
		
		public static Settings OnDeserialize() {
			AssignFromPrefs();
			
			return new Settings {
				UseLines = UseCustomLines,
				UseIcons = UseCustomIcons,
				UseMultiPrefab = EditorPrefs.GetBool(KEY_USE_CUSTOM_PREFAB, false),
			};
		}

		public static void OnSerialize(Settings settings) {
			EditorPrefs.SetBool(KEY_USE_CUSTOM_LINES, settings.UseLines);
			EditorPrefs.SetBool(KEY_USE_CUSTOM_ICONS, settings.UseIcons);
			EditorPrefs.SetBool(KEY_USE_CUSTOM_PREFAB, settings.UseMultiPrefab);

			if (settings.UseIcons != UseCustomIcons && !settings.UseIcons) {
				CoreHierarchyPass.ResetTree();
			}
			
			AssignFromPrefs();
		}
		
		public static void OnGUI(string searchContext, Settings obj) {
			GUI.enabled = !EditorApplication.isCompiling;

			if (!GUI.enabled) {
				EditorGUILayout.HelpBox("The Editor is currently recompiling...", MessageType.Info);
			}
			
			EditorGUI.indentLevel++;
			EditorGUILayout.HelpBox("Shows custom line connectors across scene objects.", MessageType.Info);
			obj.UseLines = EditorGUILayout.ToggleLeft(_useCustomLinesText, obj.UseLines);
			EditorGUILayout.HelpBox("Shows icons on scene objects that reflect object state.", MessageType.Info);
			obj.UseIcons = EditorGUILayout.ToggleLeft(_useCustomIconsText, obj.UseIcons);
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.HelpBox("Shows a context option for creating multiple prefabs from a multi-selection in \"Prefab/Multi-Prefab\".", MessageType.Info);
			obj.UseMultiPrefab = EditorGUILayout.ToggleLeft(_useMultiPrefabText, obj.UseMultiPrefab);
			
			if (EditorGUI.EndChangeCheck()) {
				// update preprocessors
				string definesString =
					PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
				List<string> allDefines = definesString.Split(';').ToList();
				
				bool containsMultiPrefab = allDefines.Contains(DEF_PREFAB);

				if (containsMultiPrefab && !obj.UseMultiPrefab) {
					allDefines.Remove(DEF_PREFAB);
				} else if (!containsMultiPrefab && obj.UseMultiPrefab) {
					allDefines.Add(DEF_PREFAB);
				}
						
				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines));
				AssetDatabase.Refresh();
				
				OnSerialize(obj);
			}
			
			EditorGUI.indentLevel--;
		}
		
		internal sealed class Settings {
			public bool UseLines;
			public bool UseIcons;
			public bool UseMultiPrefab;
		}
	}
}