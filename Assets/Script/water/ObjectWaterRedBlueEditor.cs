using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectWaterRedBlue))]
public class ObjectWaterRedBlueEditor : Editor
{
    private ObjectWaterRedBlue m_water;

    private SerializedProperty m_damage;
    private SerializedProperty m_size;
    private SerializedProperty m_color;
    private SerializedProperty m_muteSound;

    void OnEnable()
    {
        m_water = target as ObjectWaterRedBlue;

        m_size = serializedObject.FindProperty("m_size");
        m_damage = serializedObject.FindProperty("m_damage");
        m_color = serializedObject.FindProperty("m_color");
        m_muteSound = serializedObject.FindProperty("m_muteSound");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_color);
        EditorGUILayout.PropertyField(m_damage);
        EditorGUILayout.PropertyField(m_muteSound);
        EditorGUI.BeginChangeCheck();
        Vector2 newSizeValue = EditorGUILayout.Vector2Field("Size", m_size.vector2Value);
        if (newSizeValue.x > 0f && newSizeValue.y > 0f)
            m_size.vector2Value = newSizeValue;

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            m_water.AdjustComponentSizes();
            EditorUtility.SetDirty(target);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
