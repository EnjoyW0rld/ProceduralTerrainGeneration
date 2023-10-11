using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(HideIfAttribute))]
public class HideIfEditor : PropertyDrawer
{
    private bool show;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (show)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
            //return base.GetPropertyHeight(property, label);
        }

        else return 0;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        HideIfAttribute hideIf = attribute as HideIfAttribute;
        show = Compare(property.serializedObject.FindProperty(hideIf.targetName), hideIf.value, hideIf.comparison);

        if (show)
        {
            EditorGUI.PropertyField(position, property, label,true);
            //Debug.Log(property.depth);
        }
    }

    private bool Compare(SerializedProperty target, object comparer, HideIfAttribute.Comparison compareType)
    {
        SerializedPropertyType propType = target.propertyType;
        try
        {
            switch (propType)
            {
                case SerializedPropertyType.Boolean:
                    return CompareBool(target.boolValue, (bool)comparer, compareType);
                case SerializedPropertyType.Enum:
                    return CompareEnum(target.enumValueFlag, (int)comparer, compareType);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Types of two variables are not the same");
        }
        return false;
    }
    private bool CompareBool(bool first, bool second, HideIfAttribute.Comparison compareType)
    {
        switch (compareType)
        {
            case HideIfAttribute.Comparison.Equals:
                return first == second;
            case HideIfAttribute.Comparison.NotEquals:
                return first != second;
        }
        Debug.LogError("Wrong comparer for bool!");
        return false;
    }
    private bool CompareEnum(int first, int second,HideIfAttribute.Comparison compareType)
    {
        switch (compareType)
        {
            case HideIfAttribute.Comparison.Equals:
                return first != second;
            case HideIfAttribute.Comparison.NotEquals:
                return first == second;
        }
        Debug.LogError("Wrong comparison type for enums!");
        return false;
    }
}
