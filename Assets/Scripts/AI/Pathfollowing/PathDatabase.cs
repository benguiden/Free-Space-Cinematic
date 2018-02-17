using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FreeSpace{

	[CreateAssetMenu(fileName = "Path Database", menuName = "Database/Path", order = 0)]
	public class PathDatabase : ScriptableObject {

		public List<Path> paths;

	}
		
	[System.Serializable]
	public class Path{
		public List<Vector3> points;

		public Path(){
			Debug.Log ("New Point");
			points = new List<Vector3> ();
			points.Add (new Vector3 ());
		}
	}

}


