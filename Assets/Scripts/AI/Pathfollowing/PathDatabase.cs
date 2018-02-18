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
        public string name;
        public Color color;
		public List<Vector3> points;

		public Path(){
            name = "New Path";
			points = new List<Vector3> ();
            color = Color.white;
		}
	}

}


