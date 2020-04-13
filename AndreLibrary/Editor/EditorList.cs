using System;
using UnityEditor;
using UnityEngine;

[Flags]
public enum EditorListOption
{
    None = 1 << 0,
    ListSize = 1 << 1,
    ListLabel = 1 << 2,
    ElementLabels = 1 << 3,
    Buttons = 1 << 4,
    Default = ListSize | ListLabel | ElementLabels,
    NoElementLabels = ListSize | ListLabel,
    All = Default | Buttons
}

[Flags]
public enum EditorListButtonOption
{
    AddAndRemove = 1 << 0,
    Move = 1 << 1,
    All = AddAndRemove | Move
}

public static class EditorList
{
    private static readonly GUILayoutOption miniButtonWidth = GUILayout.Width(20f);
    private static readonly GUIContent
     moveButtonUpContentNull = new GUIContent(" ", "move up"),
     moveButtonUpContent = new GUIContent("\u21d1", "move up"),
     moveButtonDownContent = new GUIContent("\u21d3", "move down"),
     duplicateButtonContent = new GUIContent("+", "duplicate"),
     deleteButtonContent = new GUIContent("-", "delete"),
     addButtonContent = new GUIContent("+", "add element");

    public static void Show(SerializedProperty list, EditorListOption options = EditorListOption.Default, EditorListButtonOption buttons = EditorListButtonOption.All)
    {
        if (!list.isArray)
        {
            EditorGUILayout.HelpBox(list.name + " is neither an array nor a list!", MessageType.Error);
            return;
        }

        bool
            showListLabel = (options & EditorListOption.ListLabel) != 0,
            showListSize = (options & EditorListOption.ListSize) != 0;

        if (showListLabel)
        {
            EditorGUILayout.PropertyField(list);
            EditorGUI.indentLevel += 1;
        }
        if (!showListLabel || list.isExpanded)
        {
            SerializedProperty size = list.FindPropertyRelative("Array.size");
            if (showListSize)
            {
                EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
            }
            if (size.hasMultipleDifferentValues)
            {
                EditorGUILayout.HelpBox("Not showing lists with different sizes.", MessageType.Info);
            }
            else
            {
                ShowElements(list, options, buttons);
            }
        }
        if (showListLabel)
        {
            EditorGUI.indentLevel -= 1;
        }
    }

    private static void ShowElements(SerializedProperty list, EditorListOption options, EditorListButtonOption buttons = EditorListButtonOption.All)
    {
        bool
            showElementLabels = (options & EditorListOption.ElementLabels) != 0,
            showButtons = (options & EditorListOption.Buttons) != 0;

        for (int i = 0; i < list.arraySize; i++)
        {
            if (showButtons)
            {
                EditorGUILayout.BeginHorizontal();
            }
            if (showElementLabels)
            {
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
            }
            else
            {
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
            }
            if (showButtons)
            {
                ShowButtons(list, i, buttons);
                EditorGUILayout.EndHorizontal();
            }
        }
        if (showButtons && list.arraySize == 0 && GUILayout.Button(addButtonContent, EditorStyles.miniButton))
        {
            list.arraySize += 1;
        }
    }

    private static void ShowButtons(SerializedProperty list, int index, EditorListButtonOption buttons = EditorListButtonOption.All)
    {
        bool
           showMoveButtons = (buttons & EditorListButtonOption.Move) != 0,
           showAddButtons = (buttons & EditorListButtonOption.AddAndRemove) != 0;

        if (showMoveButtons && showAddButtons)
        {
            if (GUILayout.Button(moveButtonUpContent, EditorStyles.miniButtonLeft, miniButtonWidth))
            {
                if (index > 0)
                {
                    list.MoveArrayElement(index, index - 1);
                }
            }
            if (GUILayout.Button(moveButtonDownContent, EditorStyles.miniButtonMid, miniButtonWidth))
            {
                list.MoveArrayElement(index, index + 1);
            }
            if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonMid, miniButtonWidth))
            {
                list.InsertArrayElementAtIndex(index);
            }
            if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
            {
                int oldSize = list.arraySize;
                list.DeleteArrayElementAtIndex(index);
                if (list.arraySize == oldSize)
                {
                    list.DeleteArrayElementAtIndex(index);
                }
            }
        }
        else if (showMoveButtons)
        {
            if (index == 0)
            {
                if (GUILayout.Button(moveButtonUpContentNull, EditorStyles.miniButtonLeft, miniButtonWidth))
                {
                   
                }
            }
            else
            {
                if (GUILayout.Button(moveButtonUpContent, EditorStyles.miniButtonLeft, miniButtonWidth))
                {
                    list.MoveArrayElement(index, index - 1);
                }
            }

            if (GUILayout.Button(moveButtonDownContent, EditorStyles.miniButtonRight, miniButtonWidth))
            {
                list.MoveArrayElement(index, index + 1);
            }
        }
        else
        {
            if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
            {
                list.InsertArrayElementAtIndex(index);
            }
            if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
            {
                int oldSize = list.arraySize;
                list.DeleteArrayElementAtIndex(index);
                if (list.arraySize == oldSize)
                {
                    list.DeleteArrayElementAtIndex(index);
                }
            }
        }
    }
}
