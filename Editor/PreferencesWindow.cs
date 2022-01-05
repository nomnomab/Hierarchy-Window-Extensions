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
		
		private const string KEY_USE_CUSTOM_LINES = "nomnom.hierarchy-window-extensions.use-lines";
		private const string KEY_USE_CUSTOM_ICONS = "nomnom.hierarchy-window-extensions.use-icons";
		
		private static GUIContent _useCustomLinesText = new GUIContent("Use Custom Lines");
		private static GUIContent _useCustomIconsText = new GUIContent("Use Custom Icons");
		
		[SettingsProvider]
		public static SettingsProvider CreateProvider() => CustomPreferences.GetProvider(typeof(PreferencesWindow));
		
		public static Settings OnDeserialize() {
			return new Settings {
				UseLines = EditorPrefs.GetBool(KEY_USE_CUSTOM_LINES, false),
				UseIcons = EditorPrefs.GetBool(KEY_USE_CUSTOM_ICONS, false),
			};
		}

		public static void OnSerialize(Settings settings) {
			EditorPrefs.SetBool(KEY_USE_CUSTOM_LINES, settings.UseLines);
			EditorPrefs.SetBool(KEY_USE_CUSTOM_ICONS, settings.UseIcons);
		}
		
		public static void OnGUI(string searchContext, Settings obj) {
			EditorGUI.indentLevel++;
			obj.UseLines = EditorGUILayout.ToggleLeft(_useCustomLinesText, obj.UseLines);
			obj.UseIcons = EditorGUILayout.ToggleLeft(_useCustomIconsText, obj.UseIcons);
			EditorGUI.indentLevel--;

			if (GUILayout.Button("Apply")) {
				OnSerialize(obj);
						
				// update preprocessors
				string definesString =
					PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
				List<string> allDefines = definesString.Split(';').ToList();

				bool containsLines = allDefines.Contains(DEF_LINES);
				bool containsIcons = allDefines.Contains(DEF_ICONS);
						
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
						
				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines));
				AssetDatabase.Refresh();
			}
		}
		
		internal sealed class Settings {
			public bool UseLines;
			public bool UseIcons;
		}
	}
}