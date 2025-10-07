using UnityEditor;
using UnityEditorInternal;
using UnityEngine;



[CustomEditor(typeof(GOAPActionSet))]
public class GOAPActionSetEditor : Editor
{
    private SerializedProperty actionsProp;
    private SerializedProperty goalsProp;
    private SerializedProperty worldStateProfileProp;

    private ReorderableList actionsList;
    private ReorderableList goalsList;
    private ReorderableList seedsList;

    private void OnEnable()
    {
        actionsProp = serializedObject.FindProperty("actions");
        goalsProp = serializedObject.FindProperty("goals");
        worldStateProfileProp = serializedObject.FindProperty("worldStateProfile");

        DrawActionsList();
        DrawGoalsList();
        DrawSeedsList();

    }

    private void DrawSeedsList()
    {
        seedsList = new ReorderableList(serializedObject, worldStateProfileProp, true, true, true, true);
        seedsList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "World State Profile");

        // Give each entry more space depending on type
        seedsList.elementHeightCallback = index =>
        {
            var element = worldStateProfileProp.GetArrayElementAtIndex(index);
            var typeProp = element.FindPropertyRelative("type");

            float line = EditorGUIUtility.singleLineHeight + 4f; // one line + padding
            float total = line * 3; // key + type + value

            // Vector3 needs extra height
            if ((GOAPValueType)typeProp.enumValueIndex == GOAPValueType.Vector3)
                total += line; // one more line for the vector field

            return total;
        };

        seedsList.drawElementCallback = (rect, index, active, focused) =>
        {
            var element = worldStateProfileProp.GetArrayElementAtIndex(index);
            var keyProp = element.FindPropertyRelative("key");
            var typeProp = element.FindPropertyRelative("type");

            float line = EditorGUIUtility.singleLineHeight;
            float pad = 2f;

            var r = new Rect(rect.x, rect.y, rect.width, line);

            EditorGUI.PropertyField(r, keyProp);
            r.y += line + pad;
            EditorGUI.PropertyField(r, typeProp);

            r.y += line + pad;
            switch ((GOAPValueType)typeProp.enumValueIndex)
            {
                case GOAPValueType.Bool:
                    EditorGUI.PropertyField(r, element.FindPropertyRelative("boolValue"));
                    break;
                case GOAPValueType.Int:
                    EditorGUI.PropertyField(r, element.FindPropertyRelative("intValue"));
                    break;
                case GOAPValueType.Float:
                    EditorGUI.PropertyField(r, element.FindPropertyRelative("floatValue"));
                    break;
                case GOAPValueType.Vector3:
                    EditorGUI.PropertyField(r, element.FindPropertyRelative("vector3Value"));
                    break;
                case GOAPValueType.Transform:
                    EditorGUI.PropertyField(r, element.FindPropertyRelative("transformValue"));
                    break;
            }
        };
    }

    private void DrawGoalsList()
    {
        goalsList = new ReorderableList(serializedObject, goalsProp, true, true, true, true);
        goalsList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Goals");

        goalsList.elementHeightCallback = index =>
        {
            var element = goalsProp.GetArrayElementAtIndex(index);
            var typeProp = element.FindPropertyRelative("type");

            float line = EditorGUIUtility.singleLineHeight + 4f;
            float total = line * 3; // key + type + value

            if ((GOAPValueType)typeProp.enumValueIndex == GOAPValueType.Vector3)
                total += line;

            return total;
        };

        goalsList.drawElementCallback = (rect, index, active, focused) =>
        {
            var element = goalsProp.GetArrayElementAtIndex(index);
            var keyProp = element.FindPropertyRelative("key");
            var typeProp = element.FindPropertyRelative("type");

            float line = EditorGUIUtility.singleLineHeight;
            float pad = 2f;

            var r = new Rect(rect.x, rect.y, rect.width, line);

            EditorGUI.PropertyField(r, keyProp);
            r.y += line + pad;
            EditorGUI.PropertyField(r, typeProp);

            r.y += line + pad;
            switch ((GOAPValueType)typeProp.enumValueIndex)
            {
                case GOAPValueType.Bool:
                    EditorGUI.PropertyField(r, element.FindPropertyRelative("boolValue"));
                    break;
                case GOAPValueType.Int:
                    EditorGUI.PropertyField(r, element.FindPropertyRelative("intValue"));
                    break;
                case GOAPValueType.Float:
                    EditorGUI.PropertyField(r, element.FindPropertyRelative("floatValue"));
                    break;
                case GOAPValueType.Vector3:
                    EditorGUI.PropertyField(r, element.FindPropertyRelative("vector3Value"));
                    break;
                case GOAPValueType.Transform:
                    EditorGUI.PropertyField(r, element.FindPropertyRelative("transformValue"));
                    break;
            }
        };
    }

    private void DrawActionsList()
    {
        actionsList = new ReorderableList(serializedObject, actionsProp, true, true, true, true);
        actionsList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Actions");
        actionsList.drawElementCallback = (rect, index, active, focused) =>
        {
            var element = actionsProp.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        actionsList.DoLayoutList();
        EditorGUILayout.Space();
        goalsList.DoLayoutList();
        EditorGUILayout.Space();
        seedsList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
