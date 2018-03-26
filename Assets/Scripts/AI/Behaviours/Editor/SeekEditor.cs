using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FreeSpace {

    /*[CustomPropertyDrawer(typeof(Seek))]
    public class SeekEditor:PropertyDrawer {

        bool foldOut = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            SerializedProperty enabled = property.FindPropertyRelative("enabled");
            SerializedProperty target = property.FindPropertyRelative("target");
            SerializedProperty cruiseSpeed = property.FindPropertyRelative("cruiseSpeed");
            SerializedProperty desiredArriveSpeed = property.FindPropertyRelative("desiredArriveSpeed");
            SerializedProperty nearingDistance = property.FindPropertyRelative("nearingDistance");

            EditorGUILayout.BeginHorizontal();
            foldOut = EditorGUILayout.Foldout(foldOut, "Seek");
            enabled.boolValue = EditorGUILayout.Toggle(enabled.boolValue, GUILayout.Width(32f));
            EditorGUILayout.EndHorizontal();

            if (foldOut) {
                EditorGUILayout.LabelField("Seeking", EditorStyles.boldLabel);
                EditorGUILayout.ObjectField(target, typeof(Transform));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
                cruiseSpeed.floatValue = EditorGUILayout.FloatField("Cruise Speed", cruiseSpeed.floatValue);
                desiredArriveSpeed.floatValue = EditorGUILayout.FloatField("Desired Arrive Speed", desiredArriveSpeed.floatValue);
                nearingDistance.floatValue = EditorGUILayout.FloatField("Nearing Distance", nearingDistance.floatValue);
            }
        }

    }*/
    public abstract class BoidBehaviourEditor {
        public 
    }

}