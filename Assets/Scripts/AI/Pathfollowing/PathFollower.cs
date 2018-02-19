using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace{

    public class PathFollower : MonoBehaviour{

        //Change To Job System At Some Point

        #region Public Variables
        [Header ("Path")]
        public PathDatabase pathDatabase;
        public int pathIndex = 0;
        #endregion

        #region Movement
        [Space]
        [Header ("Movement")]
        public float speed;
        public AnimationCurve curve;
        #endregion

    }
    

}