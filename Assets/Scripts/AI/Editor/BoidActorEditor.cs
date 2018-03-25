using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FreeSpace {

    [CustomEditor(typeof(BoidActor))]
    public class BoidActorEditor : Editor {
        
        public override void OnInspectorGUI() {
            BoidActor boidActor = (BoidActor)target;

            DrawDefaultInspector();


            SerializedProperty behavioursProperties = serializedObject.FindProperty("behaviours");

            for (int i=0; i<boidActor.behaviours.Count; i++) {
                SerializedProperty currentBehaviour = behavioursProperties.GetArrayElementAtIndex(i);
                
                EditorGUILayout.PropertyField(currentBehaviour);
                EditorGUILayout.Space();
            }

            var childEnum = serializedObject.FindProperty("behaviours").GetEnumerator();
            while (childEnum.MoveNext()) {
                var current = childEnum.Current as SerializedProperty;
               // EditorGUILayout.PropertyField(current);
            }

            if (GUILayout.Button("Add Seek")) {
                boidActor.behaviours.Add(new Seek(boidActor));
            }else if (GUILayout.Button("Add Path")) {
                boidActor.behaviours.Add(new PathFollower(boidActor));
            }
        }

    }

}
