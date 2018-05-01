using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    [AddComponentMenu ("Boid Behaviours/Offset Persue")]
    [RequireComponent(typeof(BoidActor))]
    public class OffsetPursue : BoidBehaviour{

        #region Public Variables
        [Header ("Pursuing")]
        public BoidActor leader;
        #endregion

        #region Hidden Variables
        [HideInInspector]
        public Vector3 leaderOffset;
        #endregion

        #region Private Variables
        private Vector3 desiredPosition;
        private float desiredSpeed;
        #endregion

        #region Mono Methods
        protected override void Awake() {
            base.Awake ();
        }

        private void Start() {
            if (leader != null)
                RefreshOffset ();
        }

        private void OnDrawGizmos() {
            if ((Application.isPlaying) && (enabled)) {
                if (boid == null)
                    boid = GetComponent<BoidActor> ();

                Gizmos.color = new Color (1f, 1f, 0f, 0.5f);
                Gizmos.DrawLine (transform.position, desiredPosition);
                Gizmos.DrawWireSphere (desiredPosition, 2.5f);
            }
        }
        #endregion

        #region Seek Methods
        public void RefreshOffset() {
            if (leader != null) {
                leaderOffset = transform.position - leader.transform.position;
                leaderOffset = Quaternion.Inverse (leader.transform.rotation) * leaderOffset;
            }
        }

        public override Vector3 UpdateForce() {
            if (leader != null) {
                return boid.SeekForce (desiredPosition, desiredSpeed);
            } else {
                return Vector3.zero;
            }
        }

        protected override void Calculate() {
            if (leader != null) {
                desiredSpeed = boid.GetArriveSpeed (leader.transform.position, leaderOffset.magnitude, leader.speed, boid.maxSpeed, true);

                desiredPosition = leader.transform.TransformPoint (leaderOffset);

                float timeToTarget = Vector3.Distance (transform.position, leader.transform.position) / desiredSpeed;
                desiredPosition = desiredPosition + (timeToTarget * leader.velocity);
            }
        }
        #endregion

    }

}