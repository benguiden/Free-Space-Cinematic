using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FreeSpace {

    public class BoidBehaviourEditor {
        public static BoidBehaviour InspectorGUI(BoidBehaviour behaviour) {
            System.Type behaviourType = behaviour.GetType ();
            if (behaviourType == typeof (Seek)) {
                return InspectorGUI ((Seek)behaviour);
            }else if (behaviourType == typeof (PathFollower)) {
                return InspectorGUI ((PathFollower)behaviour);
            }
            return behaviour;
        }

        public static Seek InspectorGUI(Seek seek) {
            EditorGUILayout.LabelField ("Seek", EditorStyles.boldLabel);
            seek.target = (Transform)EditorGUILayout.ObjectField (seek.target, typeof (Transform), true);

            EditorGUILayout.Space ();
            EditorGUILayout.LabelField ("Movement", EditorStyles.boldLabel);
            seek.cruiseSpeed = EditorGUILayout.FloatField ("Cruise Speed", seek.cruiseSpeed);
            seek.desiredArriveSpeed = EditorGUILayout.FloatField ("Desired Arrive Speed", seek.desiredArriveSpeed);
            seek.nearingDistance = EditorGUILayout.FloatField ("Nearing Distance", seek.nearingDistance);

            EditorGUILayout.Space ();
            return seek;
        }

        public static PathFollower InspectorGUI(PathFollower pathfollower) {
            EditorGUILayout.LabelField ("PathFollower", EditorStyles.boldLabel);

            EditorGUILayout.Space ();
            return pathfollower;
        }

        public static string GetTypeString(System.Type behaviourType) {
            if (behaviourType == typeof (Seek)) {
                return "Seek";
            }else if (behaviourType == typeof (PathFollower)) {
                return "Path Follower";
            }
            return "";
        }
    }





}