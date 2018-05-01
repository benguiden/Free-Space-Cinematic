using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    [AddComponentMenu ("Boid Behaviours/Pursue")]
    [RequireComponent (typeof (BoidActor))]
    public class Pursue : BoidBehaviour{

        #region Public Variables
        [Header ("Pursuing")]
        public BoidActor target;
        public bool slowDownAtArrive = true;

        [Header ("Movement")]
        public float desiredDistance = 50f;
        #endregion

        #region Private Variables
        private Vector3 desiredPosition;
        private float desiredSpeed;
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
        #endregion

        #region Seek Methods
        public override Vector3 UpdateForce() {
            if (target != null) {
                return boid.SeekForce (desiredPosition, desiredSpeed);
            } else {
                return Vector3.zero;
            }
        }

        protected override void Calculate() {
            if (target != null) {
                bool faceTarget = false;
                desiredSpeed = boid.GetArriveSpeed (target.transform.position, desiredDistance, target.speed, boid.maxSpeed, slowDownAtArrive, ref faceTarget);

                if (faceTarget) {
                    boid.rotationTarget = target.transform;
                }

                float timeToTarget = Vector3.Distance (transform.position, target.transform.position) / desiredSpeed;
                desiredPosition = target.transform.position + (timeToTarget * target.velocity);
            }
        }
        #endregion

    }

}
