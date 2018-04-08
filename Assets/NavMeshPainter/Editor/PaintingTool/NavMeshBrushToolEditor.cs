using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ASL.NavMeshPainter
{
    [CustomPropertyDrawer(typeof(NavMeshBrushTool))]
    public class NavMeshBrushToolEditor : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            //EditorGUILayout.FloatField()
            var size = property.FindPropertyRelative("size");
            var height = property.FindPropertyRelative("maxHeight");
            var bt = property.FindPropertyRelative("brushType");

            size.floatValue = Mathf.Max(0.001f,
                EditorGUI.FloatField(new Rect(position.x, position.y, position.width, 18),size.name, size.floatValue));
            height.floatValue = Mathf.Max(0,
               EditorGUI.FloatField(new Rect(position.x, position.y + 20, position.width, 18), height.name, height.floatValue));

            EditorGUI.PropertyField(new Rect(position.x, position.y + 40, position.width, 18), bt);

            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //return base.GetPropertyHeight(property, label);
            return 60;
        }
    }
}