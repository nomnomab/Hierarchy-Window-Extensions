using Nomnom.EasierCustomPreferences.Editor;
using UnityEditor;

namespace Nomnom.HierarchyWindowExtensions.Editor {
	[PreferencesName("Hierarchy Window Extensions")]
	internal static class PreferencesWindow {
		[SettingsProvider]
		public static SettingsProvider CreateProvider() => CustomPreferences.GetProvider(typeof(PreferencesWindow));
		
		// gives the panel an object to modify
		public static Settings OnDeserialize() {
			return new Settings();
		}

		// applies the new changes to disk
		public static void OnSerialize(Settings obj) {
			
		}

		// draws whatever inside of the preferences window
		public static void OnGUI(string searchContext, Settings obj) {
			
		}
		
		public class Settings {
			
		}
	}
}