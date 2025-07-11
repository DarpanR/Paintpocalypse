using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(StatModData))]
public class StatModDataEditor : Editor {
    ReorderableList list;
    string[] behaviorTypeNames;
    Type[] behaviorTypes;
    int selectedTypeIndex = 0;

    void OnEnable() {
        var statModData = (StatModData)target;

        // Get all concrete subclasses of StatModBehavior
        behaviorTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(StatModBehavior)) && !t.IsAbstract)
            .ToArray();

        behaviorTypeNames = behaviorTypes.Select(t => t.Name).ToArray();

        // Initialize the reorderable list
        list = new ReorderableList(serializedObject,
            serializedObject.FindProperty("statMods"),
            true, true, true, true);

        list.drawHeaderCallback = rect => {
            EditorGUI.LabelField(rect, "Stat Mod Behaviors");
        };

        list.drawElementCallback = (rect, index, active, focused) => {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, true);
        };

        list.elementHeightCallback = index => {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, true) + 4f;
        };

        list.onAddCallback = l => {
            if (behaviorTypes.Length == 0)
                return;
            var newInstance = Activator.CreateInstance(behaviorTypes[selectedTypeIndex]);
            statModData.statMods.Add((StatModBehavior)newInstance);
            EditorUtility.SetDirty(statModData);
        };
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        // Draw default fields except for the statMods list
        DrawPropertiesExcluding(serializedObject, "statMods");

        GUILayout.Space(10);
        GUILayout.Label("Add New Modifier", EditorStyles.boldLabel);

        // Dropdown and add button
        EditorGUILayout.BeginHorizontal();
        selectedTypeIndex = EditorGUILayout.Popup(selectedTypeIndex, behaviorTypeNames);
        if (GUILayout.Button("Add")) {
            list.onAddCallback(list);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);
        list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}