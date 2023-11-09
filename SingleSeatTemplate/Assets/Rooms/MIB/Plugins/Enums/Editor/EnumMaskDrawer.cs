using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumMaskAttribute))]
public class EnumMaskDrawer : PropertyDrawer
{
    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //{
    //	EnumMaskAttribute flagSettings = (EnumMaskAttribute)attribute;
    //	Enum targetEnum = (Enum)fieldInfo.GetValue(property.serializedObject.targetObject);

    //	string propName = flagSettings.name;
    //	if (string.IsNullOrEmpty (propName))
    //	{ propName = ObjectNames.NicifyVariableName (property.name); }

    //	EditorGUI.BeginProperty(position, label, property);
    //	Enum enumNew = EditorGUI.EnumMaskPopup(position, propName, targetEnum);
    //	property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());
    //	EditorGUI.EndProperty();
    //}

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();
        uint a = (uint)(EditorGUI.MaskField(position, label, property.intValue, property.enumNames));
        if (EditorGUI.EndChangeCheck())
        {
            property.intValue = (int)a;
        }
    }
}