using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace DreamscapeUtil {
    [CustomEditor(typeof(AnimatorUtil))]
    [CanEditMultipleObjects]
    public class AnimatorUtilInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            AnimatorUtil animUtil = targets[0] as AnimatorUtil;

            SerializedProperty disableCullingOnServerProp = serializedObject.FindProperty("disableCullingOnServer");

            EditorGUILayout.PropertyField(disableCullingOnServerProp);

            bool setStateMixed = false;
            bool stateNameMixed = false;
            bool normalizedTimeMixed = false;
            bool randomizeTimeMixed = false;
            bool layerIndexMixed = false;
            //check for mixed values between mutliple instances
            for(int i = 1; i < targets.Length; i++)
            {
                AnimatorUtil a = targets[i] as AnimatorUtil;
                if(a.setState != animUtil.setState)
                {
                    setStateMixed = true;
                }
                if(a.stateName != animUtil.stateName)
                {
                    stateNameMixed = true;
                }
                if(a.normalizedTime != animUtil.normalizedTime)
                {
                    normalizedTimeMixed = true;
                }
                if(a.randomizeTime != animUtil.randomizeTime)
                {
                    randomizeTimeMixed = true;
                }
                if(a.layerIndex != animUtil.layerIndex)
                {
                    layerIndexMixed = true;
                }
            }

            EditorGUI.showMixedValue = layerIndexMixed;
            EditorGUI.BeginChangeCheck();
            int layerIndex = EditorGUILayout.IntField(new GUIContent("Layer Index", "Animator Layer affected by this component"), animUtil.layerIndex);
            if (EditorGUI.EndChangeCheck()) //if value changed, update all instances
            {
                Undo.RecordObjects(targets, "Inspector");
                for (int i = 0; i < targets.Length; i++)
                {
                    AnimatorUtil a = targets[i] as AnimatorUtil;
                    a.layerIndex = layerIndex;
                }
                foreach (Object t in targets)
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(t);
                    EditorUtility.SetDirty(t);
                }
            }
            EditorGUILayout.Space();

            EditorGUI.showMixedValue = setStateMixed;
            EditorGUI.BeginChangeCheck();
            bool setState = EditorGUILayout.Toggle(new GUIContent("Set Animator State", "Change animator state or play default state?"), animUtil.setState);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Inspector");
                for (int i = 0; i < targets.Length; i++)
                {
                    AnimatorUtil a = targets[i] as AnimatorUtil;
                    a.setState = setState;
                }
                foreach (Object t in targets)
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(t);
                    EditorUtility.SetDirty(t);
                }
            }
            using (new EditorGUI.DisabledGroupScope(!animUtil.setState && !setStateMixed)) {
                using (new EditorGUI.IndentLevelScope()) {
                    EditorGUI.showMixedValue = stateNameMixed;
                    EditorGUI.BeginChangeCheck();
                    string stateName = EditorGUILayout.TextField(new GUIContent("State Name", "Animator state to play"), animUtil.stateName);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObjects(targets, "Inspector");
                        for (int i = 0; i < targets.Length; i++)
                        {
                            AnimatorUtil a = targets[i] as AnimatorUtil;
                            a.stateName = stateName;
                        }
                        foreach (Object t in targets)
                        {
                            PrefabUtility.RecordPrefabInstancePropertyModifications(t);
                            EditorUtility.SetDirty(t);
                        }
                    }
                }
            }
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUI.showMixedValue = normalizedTimeMixed;
            GUI.enabled = !animUtil.randomizeTime || randomizeTimeMixed;
            EditorGUI.BeginChangeCheck();
            float normalizedTime = EditorGUILayout.FloatField(new GUIContent("Normalized Time", "Time to start animation clip (0-1)"), animUtil.normalizedTime);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Inspector");
                for (int i = 0; i < targets.Length; i++)
                {
                    AnimatorUtil a = targets[i] as AnimatorUtil;
                    a.normalizedTime = normalizedTime;
                }
                foreach (Object t in targets)
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(t);
                    EditorUtility.SetDirty(t);
                }
            }
            GUI.enabled = true;
            EditorGUI.showMixedValue = randomizeTimeMixed;
            EditorGUI.BeginChangeCheck();
            bool randomizeTime = EditorGUILayout.ToggleLeft(new GUIContent("Randomize", "Start animation at a random time in the clip"), animUtil.randomizeTime);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Inspector");
                for (int i = 0; i < targets.Length; i++)
                {
                    AnimatorUtil a = targets[i] as AnimatorUtil;
                    a.randomizeTime = randomizeTime;
                }
                foreach (Object t in targets)
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(t);
                    EditorUtility.SetDirty(t);
                }
            }
            EditorGUILayout.EndHorizontal();

            
            EditorGUILayout.Space();

            GUILayout.BeginVertical(new GUIContent("Animator Parameters", "parameters will be set on start"),
                "window");

            Animator anim = animUtil.GetComponent<Animator>();
            RuntimeAnimatorController c = anim.runtimeAnimatorController;
            AnimatorController controller = c == null ? null : AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(c));

            bool mixedControllers = false;
            for (int i = 1; i < targets.Length; i++)
            {
                RuntimeAnimatorController rac = (targets[i] as AnimatorUtil).GetComponent<Animator>().runtimeAnimatorController;
                AnimatorController ac = rac == null ? null : AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(rac));
                if (ac != controller)
                {
                    mixedControllers = true;
                    break;
                }
            }

            if (mixedControllers)
            {
                EditorGUILayout.LabelField("Cannot multi-edit parameters for animators with different controllers");
            }
            else if (!controller)
            {
                EditorGUILayout.LabelField("No Animator Controller selected");
            }
            else
            {
                foreach (AnimatorControllerParameter controllerParam in controller.parameters)
                {
                    ParameterGUI(controllerParam, targets);
                }
            }

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private static void ParameterGUI(AnimatorControllerParameter controllerParam, Object[] targets)
        {
            AnimatorUtil animUtil = targets[0] as AnimatorUtil;


            GUIStyle s = new GUIStyle();
            s.alignment = TextAnchor.MiddleRight;
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            EditorGUILayout.SelectableLabel(controllerParam.name, GUILayout.Width(100));
            AnimatorParamInfo p = animUtil.parameters.Find(param => param.name == controllerParam.name);
            
            EditorGUILayout.BeginHorizontal(s, GUILayout.ExpandWidth(true));
            EditorGUI.BeginChangeCheck();
            int selection = GUILayout.SelectionGrid(p == null ? 0 : 1, new GUIContent[]
            {
                        new GUIContent("Default", "Do not set this parameter on start"),
                        new GUIContent("Set Param", "Set this parameter on start")
            }, 2, GUILayout.ExpandWidth(false));


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Inspector");
                if (selection == 0)
                {
                    //animUtil.parameters.Remove(controllerParam.name);
                    foreach (Object o in targets)
                    {
                        (o as AnimatorUtil).parameters.RemoveAll(param => param.name == controllerParam.name);
                    }
                }
                else
                {
                    foreach (Object o in targets)
                    {
                        p = new AnimatorParamInfo();
                        p.type = controllerParam.type;
                        p.name = controllerParam.name;
                        p.floatVal = controllerParam.defaultFloat;
                        p.intVal = controllerParam.defaultInt;
                        p.boolVal = controllerParam.defaultBool;
                        (o as AnimatorUtil).parameters.Add(p);
                    }
                }
                foreach (Object t in targets)
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(t);
                    EditorUtility.SetDirty(t);
                }
            }

            AnimatorParamInfo newVal = null;
            if (p != null)
            {
                newVal = new AnimatorParamInfo();
                newVal.type = p.type;
                newVal.name = p.name;
                newVal.boolVal = p.boolVal;
                newVal.floatVal = p.floatVal;
                newVal.intVal = p.intVal;
            }

            EditorGUI.BeginChangeCheck();
            if (controllerParam.type == AnimatorControllerParameterType.Bool)
            {
                if (p != null)
                {
                    EditorGUI.showMixedValue = false;
                    for(int i = 1; i < targets.Length; i++)
                    {
                        AnimatorUtil u = targets[i] as AnimatorUtil;
                        AnimatorParamInfo info = u.parameters.Find(x => x.name == p.name);
                        if(info == null || info.boolVal != p.boolVal)
                        {
                            EditorGUI.showMixedValue = true;
                        }
                    }
                    newVal.boolVal = EditorGUILayout.Toggle(p.boolVal);
                }
                else using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.Toggle(controllerParam.defaultBool);
                }
            }
            else if (controllerParam.type == AnimatorControllerParameterType.Float)
            {
                if (p != null)
                {
                    EditorGUI.showMixedValue = false;
                    for (int i = 1; i < targets.Length; i++)
                    {
                        AnimatorUtil u = targets[i] as AnimatorUtil;
                        AnimatorParamInfo info = u.parameters.Find(x => x.name == p.name);
                        if (info == null || info.floatVal != p.floatVal)
                        {
                            EditorGUI.showMixedValue = true;
                        }
                    }
                    newVal.floatVal = EditorGUILayout.FloatField(p.floatVal);
                }
                else using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.FloatField(controllerParam.defaultFloat);
                }
            }
            else if (controllerParam.type == AnimatorControllerParameterType.Int)
            {
                if (p != null)
                {
                    EditorGUI.showMixedValue = false;
                    for (int i = 1; i < targets.Length; i++)
                    {
                        AnimatorUtil u = targets[i] as AnimatorUtil;
                        AnimatorParamInfo info = u.parameters.Find(x => x.name == p.name);
                        if (info == null || info.intVal != p.intVal)
                        {
                            EditorGUI.showMixedValue = true;
                        }
                    }
                    newVal.intVal = EditorGUILayout.IntField(p.intVal);
                }
                else using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.IntField(controllerParam.defaultInt);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Inspector");

                foreach (Object t in targets)
                {
                    AnimatorParamInfo info = (t as AnimatorUtil).parameters.Find(x => x.name == controllerParam.name);

                    if (info == null)
                    {
                        info = new AnimatorParamInfo();
                        (t as AnimatorUtil).parameters.Add(info);
                    }
                    info.type = newVal.type;
                    info.name = newVal.name;
                    info.boolVal = newVal.boolVal;
                    info.floatVal = newVal.floatVal;
                    info.intVal = newVal.intVal;


                    PrefabUtility.RecordPrefabInstancePropertyModifications(t);
                    EditorUtility.SetDirty(t);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
            EditorGUI.showMixedValue = false;
        }
    }
}
