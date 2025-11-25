using CoffeyUtils;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LevelableFloat))]
public class LevelableFloatDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        int originalIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        SerializedProperty isLevelableProp = property.FindPropertyRelative("isLevelable");
        SerializedProperty levelsProp = property.FindPropertyRelative("levels");

        if (levelsProp.arraySize == 0) levelsProp.arraySize = 3;

        float toggleWidth = 20f;
        float gap = 5f;

        Rect toggleRect = new Rect(contentPosition.x, contentPosition.y, toggleWidth, contentPosition.height);
        Rect floatsRect = new Rect(contentPosition.x + toggleWidth, contentPosition.y, contentPosition.width - toggleWidth, contentPosition.height);

        EditorGUI.BeginChangeCheck();
        bool oldMixed = EditorGUI.showMixedValue;
        EditorGUI.showMixedValue = isLevelableProp.hasMultipleDifferentValues;

        bool newLevelable = EditorGUI.Toggle(toggleRect, isLevelableProp.boolValue);

        EditorGUI.showMixedValue = oldMixed;
        if (EditorGUI.EndChangeCheck())
        {
            isLevelableProp.boolValue = newLevelable;
        }

        bool isChecked = isLevelableProp.boolValue;

        int countToShow = isChecked ? levelsProp.arraySize : 1;

        float widthPerField = (floatsRect.width - (gap * (countToShow - 1))) / countToShow;

        for (int i = 0; i < countToShow; i++)
        {
            if (i >= levelsProp.arraySize) break;

            SerializedProperty element = levelsProp.GetArrayElementAtIndex(i);

            Rect fieldRect = new Rect(
                floatsRect.x + (widthPerField + gap) * i,
                floatsRect.y,
                widthPerField,
                floatsRect.height
            );

            EditorGUI.PropertyField(fieldRect, element, GUIContent.none);
        }

        EditorGUI.indentLevel = originalIndent;
        EditorGUI.EndProperty();
    }
}