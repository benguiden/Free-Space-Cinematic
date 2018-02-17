using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FreeSpace {

	[CustomEditor(typeof(FreeSpace.PathDatabase))]
	public class PathDatabaseEditor : Editor {

		private int pathIndex = 0;

		private PathDatabase database;

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();
		}

		void OnEnable(){
			SceneView.onSceneGUIDelegate += OnSceneGUI;
		}

		void OnDisable(){
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
		}

		void OnSceneGUI(SceneView sceneView){

			database = (PathDatabase)target;
			Path path = database.paths [pathIndex];

			for (int i = 0; i < path.points.Count; i++) {
				Handles.DrawWireCube (path.points [i], new Vector3 (1f, 1f, 1f));
				if (i < path.points.Count - 1)
					Handles.DrawLine (path.points [i], path.points [i + 1]);
				else if (i == path.points.Count - 1)
					Handles.DrawLine (path.points [i], path.points [0]);
			}

			database.paths [pathIndex] = path;

		}

	}

}
