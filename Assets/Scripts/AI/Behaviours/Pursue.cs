using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    [RequireComponent (typeof (BoidActor))]
    public class Pursue : BoidBehaviour{

        #region Public Variables
        [Header ("Pursuing")]
        public BoidActor target;

        [Header ("Movement")]
        public float desiredDistance = 50f;
        #endregion

        #region Private Variables
        private Vector3 desiredForward = new Vector3 ();
        private float deltaSpeed;
        #endregion

    }

}
