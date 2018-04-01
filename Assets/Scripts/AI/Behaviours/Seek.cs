using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    [RequireComponent (typeof (BoidActor))]
    public class Seek : BoidBehaviour {

        #region Public Variables
        [Header("Seeking")]
        public Transform target;

        [Header ("Movement")]
        public float cruiseSpeed = 20f;
        #endregion

        #region Private Variables
        private Vector3 desiredPosition;
        #endregion

        #region Mono Methods
        protected override void Awake() {
            base.Awake ();
        }

        private void OnDrawGizmos() {
            if ((Application.isPlaying) && (enabled)) {
                if (boid == null)
                    boid = GetComponent<BoidActor> ();

                Gizmos.color = new Color (0f, 0f, 1f, 0.5f);
                Gizmos.DrawWireSphere (desiredPosition, 2.5f);
            }
        }

        public override Vector3 UpdateForce() {
            return boid.SeekForce (desiredPosition, cruiseSpeed);
        }
        #endregion

        #region Seek Methods
        protected override void Calculate() {
            desiredPosition = target.position;
        }
        #endregion

    }

}
