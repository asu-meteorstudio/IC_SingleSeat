/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using UnityEditor;
using UnityEngine;
using Leap.Unity;

namespace Leap.Unity {

  [InitializeOnLoad]
  public class LeapUnityWindow : EditorWindow {

    #region Settings & Init

    private const string WINDOW_TITLE = "Leap Motion Unity Modules";
    private static LeapUnityWindow _currentWindow = null;


#if LEAP_MOTION
    private static readonly Vector2 WINDOW_MIN_SIZE = new Vector2(600f, 600f);

    /// <summary>
    /// This editor preference marks the Leap Unity SDK as having been launched. When
    /// this is not detected, we can assume this is the first time the SDK has been
    /// imported into an editor, so we open the window for discoverability.
    /// </summary>
    private const string LEAP_UNITY_WINDOW_LAUNCHED_PREF = "Leap Unity Window Launched";
    /// <summary>
    /// This static constructor is called as the editor launches due to the
    /// InitializeOnLoad attribute on LeapUnityWindow.
    /// </summary>
    static LeapUnityWindow() {
        EditorApplication.delayCall += () => {
            if (!EditorPrefs.GetBool(LEAP_UNITY_WINDOW_LAUNCHED_PREF)) {
                EditorPrefs.SetBool(LEAP_UNITY_WINDOW_LAUNCHED_PREF, true);
                Init();
            }
        };
    }

    [MenuItem("Window/Leap Motion")]
    public static void Init() {
      var window = (LeapUnityWindow)GetWindow(typeof(LeapUnityWindow),
        utility: true, title: WINDOW_TITLE, focus: true);
      window.name = "Leap Motion Unity Modules Window";
      window.minSize = WINDOW_MIN_SIZE;
      _currentWindow = window;
    }

#endif


        #endregion

        #region Resources

        private string leapLogoResourceName {
      get {
        if (EditorGUIUtility.isProSkin) return "LM_Logo_White";
        else return "LM_Logo_Black";
      }
    }

    private Texture2D _backingLeapTex = null;
    private Texture2D leapTex {
      get {
        if (_backingLeapTex == null) {
          _backingLeapTex = EditorResources.Load<Texture2D>(leapLogoResourceName);
        }
        return _backingLeapTex;
      }
    }

    private static GUISkin s_backingWindowSkin;
    public static GUISkin windowSkin {
      get {
        if (s_backingWindowSkin == null) {
          s_backingWindowSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
        }
        return s_backingWindowSkin;
      }
    }

    #endregion

    #region Window State

    public static bool isWindowOpen {
      get {
        return _currentWindow != null;
      }
    }

    private int _tabIndex = 0;
    private static string[] _tabs = new string[] {
      "Project Checks", "Rig Upgrader", "Preferences"
    };
    public static void ShowTab(int tabIndex) {
      if (_currentWindow != null) {
        _currentWindow._tabIndex = tabIndex;
      }
    }
    public static int GetTabCount() { return _tabs.Length; }

    private Vector2 _scrollPosition = Vector2.zero;

    #endregion

    #region OnGUI

    private void OnGUI() {
      var origSkin = GUI.skin;
      try {
        GUI.skin = windowSkin;

        drawGUI();
      }
      finally {
        GUI.skin = origSkin;
      }
    }

    private void drawGUI() {
      var boxStyle = windowSkin.box;
      GUILayout.BeginVertical();

      // Logo.
      var logoStyle = windowSkin.box;
      logoStyle.fixedHeight = 150;
      logoStyle.stretchWidth = true;
      logoStyle.alignment = TextAnchor.MiddleCenter;
      logoStyle.margin = new RectOffset(0, 0, top: 20, bottom: 20);
      GUILayout.Box(new GUIContent(leapTex), logoStyle, GUILayout.ExpandWidth(true),
        GUILayout.MaxHeight(150f));

      // Window tabs.
      _tabIndex = GUILayout.Toolbar(_tabIndex, _tabs);
      _scrollPosition = GUILayout.BeginScrollView(_scrollPosition,
        GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
      switch (_tabIndex) {
        case 0:
          LeapProjectChecks.DrawProjectChecksGUI();
          break;
        case 1:
          LeapRigUpgrader.DrawUpgraderGUI();
          break;
        case 2:
          float prevLabelWidth = EditorGUIUtility.labelWidth;
          EditorGUIUtility.labelWidth = 200;
          LeapPreferences.DrawPreferencesGUI();
          EditorGUIUtility.labelWidth = prevLabelWidth;
          break;
        default:
          _tabIndex = 0;
          break;
      }
      GUILayout.EndScrollView();

      GUILayout.EndVertical();
    }

    #endregion

  }

}
