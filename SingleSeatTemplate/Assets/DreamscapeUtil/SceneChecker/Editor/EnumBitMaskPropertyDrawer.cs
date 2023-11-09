using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DreamscapeUtil
{

    [CustomPropertyDrawer(typeof(BitMaskAttribute))]
    public class EnumBitMaskPropertyDrawer : PropertyDrawer
    {

        public static int DrawBitMaskField(Rect aPosition, int aMask, System.Type aType, GUIContent aLabel)
        {
            var itemNames = System.Enum.GetNames(aType);
            var itemValues = System.Enum.GetValues(aType) as int[];

            int val = aMask;
            int maskVal = 0;
            for (int i = 0; i < itemValues.Length; i++)
            {
                if (itemValues[i] != 0)
                {
                    if ((val & itemValues[i]) == itemValues[i])
                        maskVal |= 1 << i;
                }
                else if (val == 0)
                    maskVal |= 1 << i;
            }
            int newMaskVal = EditorGUI.MaskField(aPosition, aLabel, aMask, itemNames);
            if (newMaskVal == 0)
            {
                return 0;
            }
            int changes = maskVal ^ newMaskVal;


            for (int i = 0; i < itemValues.Length; i++)
            {
                if ((changes & (1 << i)) != 0)            // has this list item changed?
                {
                    if ((newMaskVal & (1 << i)) != 0)     // has it been set?
                    {
                        val |= itemValues[i];
                    }
                    else                                  // it has been reset
                    {
                        val &= ~itemValues[i];
                    }
                }
            }

            bool everything = true;
            //need to special case when everything is selected
            for (int i = 0; i < itemValues.Length; i++)
            {
                if ((val & itemValues[i]) == 0)
                {
                    everything = false;
                    break;
                }
            }
            if (everything)
            {
                return -1;
            }
            else
            {
                return val;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var typeAttr = attribute as BitMaskAttribute;
            int before = prop.intValue;

            prop.intValue = DrawBitMaskField(position, prop.intValue, typeAttr.propType, label);
        }
    }
}
