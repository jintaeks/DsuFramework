#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Dsu.Framework
{
#if UNITY_EDITOR
    public static class LogSettings
    {
        private const string EditorPrefKey = "Dsu_LogEnabled";

        public static bool Enabled
        {
            get => EditorPrefs.GetBool(EditorPrefKey, true);
            set => EditorPrefs.SetBool(EditorPrefKey, value);
        }
    }

    public class LogSettingsWindow : EditorWindow
    {
        [MenuItem("Tools/DsuFramework/Dsu Log Settings", false, 150)]
        public static void ShowWindow()
        {
            GetWindow<LogSettingsWindow>("Dsu Log Settings");
        }

        private void OnGUI()
        {
            GUILayout.Label("Log Settings", EditorStyles.boldLabel);

            bool enabled = LogSettings.Enabled;
            bool newEnabled = EditorGUILayout.Toggle("Enable Logging", enabled);

            if (newEnabled != enabled) {
                LogSettings.Enabled = newEnabled;
            }
        }
    }
#else
    public static class LogSettings
    {
        public static bool Enabled => true; // Runtime에서는 항상 로그 출력
    }
#endif
}
