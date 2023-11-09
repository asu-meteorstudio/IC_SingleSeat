using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;

namespace DreamscapeUtil {


    public class CommentExplorerWindow : EditorWindow
    {

        private Comment[] comments;
        private Dictionary<string, Comment> collapsedComments = new Dictionary<string, Comment>();
        private Dictionary<string, int> collapsedCounts = new Dictionary<string, int>();
        private static string searchString = "";

        private Vector2 scrollPos;

        private static GUIStyle defaultStyle = null;
        private static GUIStyle selectedStyle = null;

        private bool collapse = true;

        [MenuItem("Dreamscape/Comment Explorer...")]
        static void Init()
        {

            CommentExplorerWindow window = GetWindow<CommentExplorerWindow>("Comment Explorer");
            window.Populate();

            window.Show();
        }

        private void OnFocus()
        {
            Populate();
            this.Repaint();
        }

        private void OnHierarchyChange()
        {
            Populate();
            this.Repaint();
        }

        private void Populate()
        {
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                UnityEditor.SceneManagement.PrefabStage stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                comments = stage.prefabContentsRoot.GetComponentsInChildren<Comment>(true);
            }
            else
            {
                comments = FindObjectsOfType<Comment>();
            }

            
            collapsedComments.Clear();
            foreach(Comment c in comments)
            {
                if (!collapsedComments.ContainsKey(c.commentText))
                {
                    collapsedComments[c.commentText] = c;
                    collapsedCounts[c.commentText] = 1;
                }
                else{
                    collapsedCounts[c.commentText]++;
                }
                
            }
        }

        private void OnGUI()
        {
            Populate();

            if(selectedStyle == null || selectedStyle.normal.background == null)
            {
                selectedStyle = new GUIStyle(EditorStyles.label);
                selectedStyle.normal.background = MakeTex(1, 1, new Color(.239f, .376f, .569f));
            }
            if(defaultStyle == null)
            {
                defaultStyle = new GUIStyle(EditorStyles.label);
            }


            //EditorGUIUtility.LookLikeInspector();
            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            collapse = GUILayout.Toggle(collapse, "Collapse", GUI.skin.FindStyle("ToolbarButton"), GUILayout.ExpandWidth(false));
            searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                // Remove focus if cleared
                searchString = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            


            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            IEnumerable commentCollection;
            if (collapse)
            {
                commentCollection = collapsedComments.Values;
            }
            else
            {
                commentCollection = comments;
            }

            foreach (Comment c in commentCollection) {

                if (c.name.ToLower().Contains(searchString.ToLower()) || c.commentText.ToLower().Contains(searchString.ToLower()))
                {
                    bool selected = Selection.Contains(c.gameObject);

                    Rect r = EditorGUILayout.GetControlRect();
                    
                    if(Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
                    {
                        Selection.activeGameObject = c.gameObject;
                        if (collapse)
                        {
                            List<Object> sel = new List<Object>();
                            foreach(Comment c2 in comments)
                            {
                                if(c.commentText.Equals(c2.commentText))
                                {
                                    sel.Add(c2.gameObject);
                                }
                            }
                            Selection.objects = sel.ToArray();
                        }
                        this.Repaint();
                    }
                    int num = 1;
                    if (collapse)
                    {
                        num = collapsedCounts[c.commentText];
                    }

                    EditorGUI.LabelField(r, c.name, selected ? selectedStyle : defaultStyle);
                    if(num > 1)
                    {
                        Rect r2 = new Rect(r.x + r.width - 20, r.y, 20, r.height);
                        EditorGUI.LabelField(r2, num.ToString(), selected ? selectedStyle : defaultStyle);
                    }
                    if (c.commentText.ToLower().Contains("todo"))
                    {
                        Rect r2 = new Rect(r.x + r.width - 40, r.y, 20, r.height);
                        EditorGUI.LabelField(r2, "*", selected ? selectedStyle : defaultStyle);
                    }
                    

                }
            }
            EditorGUILayout.EndScrollView();
        }


        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

    }
}
