using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArcadeVehicleController))]
public class AVCEditor : Editor
{
    private SerializedProperty gameOverKey;
    private SerializedProperty refillNosKey;
    private bool showDebugFoldout = true;

    private void OnEnable()
    {
        gameOverKey = serializedObject.FindProperty("gameOverKey");
        refillNosKey = serializedObject.FindProperty("refillNosKey");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Default properties
        DrawDefaultInspector();

        // Debug foldout
        showDebugFoldout = EditorGUILayout.Foldout(showDebugFoldout, "Debug");
        if (showDebugFoldout)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(gameOverKey);
            EditorGUILayout.PropertyField(refillNosKey);

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
