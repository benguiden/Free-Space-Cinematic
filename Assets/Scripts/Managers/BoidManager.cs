using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    public class BoidManager : MonoBehaviour {

        #region Public Variables
        private static BoidManager main;
        #endregion

        #region Hidden Variables
        [HideInInspector]
        public List<BoidActor> boidActors = new List<BoidActor> ();
        #endregion

        #region Mono Methods
        private void Awake() {
            main = this;
        }
        #endregion

        #region StaticMethods
        public static BoidManager Main() {
            if (main == null) {
                main = FindObjectOfType<BoidManager> ();

                if (main == null) {
                    Debug.LogError ("Error: No Boid Manager instance in scene.");
                    Debug.Break ();
                }
            }

            return main;
        }
        #endregion

    }

}
