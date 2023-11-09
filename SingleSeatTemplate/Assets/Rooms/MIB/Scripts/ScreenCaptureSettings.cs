#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Artanim.Tools
{
    [InitializeOnLoad]
    public static class ScreenCaptureSettings
    {
        const string MENU_ITEM = "Dreamscape/Screen Capture";
        const string ScreenCaptureKey = "ScreenCaptureKey";

        public static bool ScreenCaptureEnabled
        {
            get
            {
                return EditorPrefs.GetInt(ScreenCaptureKey).AsBool();
            }
            private set
            {
                EditorPrefs.SetInt(ScreenCaptureKey, value.AsInt());
            }
        }

        [MenuItem(MENU_ITEM)]
        private static void ToggleScreenCapture()
        {
            ScreenCaptureEnabled = !ScreenCaptureEnabled;
        }

        [MenuItem(MENU_ITEM, true)]
        private static bool ValidateToggleScreenCapture()
        {
            Menu.SetChecked(MENU_ITEM, ScreenCaptureEnabled);
            return !EditorApplication.isPlaying;
        }
    }

    public static class BoolExtensions
    {
        public static int AsInt(this bool value)
        {
            return value ? 1 : 0;
        }
    }

    public static class IntExtensions
    {
        public static bool AsBool(this int value)
        {
            return value == 0 ? false : true;
        }
    }
}
#endif