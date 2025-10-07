using UnityEditor;
using UnityEngine;
using System;


[CustomPropertyDrawer(typeof(GOAPKey))]
public class GOAPKeyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        GOAPKey current = (GOAPKey)property.enumValueIndex;

        if (EditorGUI.DropdownButton(position, new GUIContent(current.ToString()), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();

            foreach (GOAPKey key in Enum.GetValues(typeof(GOAPKey)))
            {
                string raw = key.ToString();

                bool isCategory = raw.StartsWith("__") && raw.EndsWith("__") && !raw.Contains("I__");
                bool isSubCategory = raw.StartsWith("__I__") && raw.EndsWith("__I__");

                if (isCategory)
                {
                    string clean = raw.Trim('_');

                    // Category header (bold/right aligned look)
                    menu.AddDisabledItem(new GUIContent(clean.ToUpper()));

                    // Divider line (disabled empty item)
                    menu.AddSeparator("");
                }
                else if (isSubCategory)
                {
                    string clean = raw.Replace("_", "");
                    menu.AddDisabledItem(new GUIContent("    " + clean));
                }
                else
                {
                    // Normal selectable entry
                    menu.AddItem(new GUIContent("   " + raw), key.Equals(current), () =>
                    {
                        property.enumValueIndex = (int)key;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
            }

            menu.ShowAsContext();
        }

        EditorGUI.EndProperty();
    }
}
