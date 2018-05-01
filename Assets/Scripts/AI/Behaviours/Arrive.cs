using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    [AddComponentMenu ("Boid Behaviours/Arrive")]
    [RequireComponent (typeof (BoidActor))]
    public class Arrive : BoidBehaviour
    {

        #region Public Variables
        [Header ("Arrive")]
        public Transform target;
        public Vector3 targetPosition;

        [Header ("Movement")]
        public float cruiseSpeed = 20f;
        public float nearingDistance = 0f;
        #endregion

        #region Private Variables
        private Vector3 desiredPosition;
        private float desiredSpeed;
        #endregion

        #region Mono Methods
        private void OnDrawGizmos() {
            if ((Application.isPlaying) && (enabled) && (target != null)) {
                if (boid == null)
                    boid = GetComponent<BoidActor> ();

                float arriveTime = boid.speed / boid.maxAcceleration;
                float arriveRadius = arriveTime * boid.speed;
                Gizmos.color = Gizmos.color = new Color (0f, 0f, 1f, 0.5f);
                Gizmos.DrawWireSphere (target.position, arriveRadius);
            }
        }

        public override Vector3 UpdateForce() {
            return boid.SeekForce (desiredPosition, desiredSpeed);
        }
        #endregion

        #region Seek Methods
        protected override void Calculate() {
            if (target != null)
                desiredPosition = target.position;
            else
                desiredPosition = targetPosition;

            bool faceTarget = false;
            desiredSpeed = boid.GetArriveSpeed (desiredPosition, nearingDistance, 0f, cruiseSpeed, false, ref faceTarget);

            if (faceTarget) {
                boid.rotationTarget = target.transform;
            }
        }
        #endregion

    }

}
