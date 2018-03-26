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
        public float desiredArriveSpeed = 20f;
        public float nearingDistance = 0f;
        #endregion

        #region Mono Methods
        protected override void Awake() {
            base.Awake ();
        }

        private void OnDrawGizmos() {
            if (Application.isPlaying) {
                if (boid == null)
                    boid = GetComponent<BoidActor> ();

                float timeToDesired = (boid.speed - desiredArriveSpeed) / boid.maxAcceleration;
                float radius = timeToDesired * boid.speed;
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere (target.position, radius);
            }
        }

        private void Update() {
            MoveTowardTarget();
        }
        #endregion

        #region Seek Methods
        
        private void MoveTowardTarget() {
            float distanceToTarget = Vector3.Distance (boid.transform.position, target.position);

            Vector3 desiredForward = (target.position - boid.transform.position).normalized;
            if (distanceToTarget > 1f)
                boid.SpinToTargetForward (desiredForward, 0.25f * Time.deltaTime * 15f);

            
            float arriveTime = (boid.speed - desiredArriveSpeed) / boid.maxAcceleration;
            float arriveRadius = arriveTime * boid.speed;

            float desiredSpeed = cruiseSpeed;
            if (distanceToTarget <= arriveRadius + nearingDistance) {
                desiredSpeed = 0f;
            } else if (distanceToTarget <= arriveRadius) {
                desiredSpeed *= distanceToTarget / (arriveRadius + nearingDistance);
            }

            float dot = 1f;
            if (boid.speed > 0f)
                dot = Vector3.Dot (boid.GetNetVelocity ().normalized, desiredForward.normalized);
            float newSpeed = Mathf.Abs ((desiredSpeed * dot) - boid.speed);
            if (desiredSpeed < boid.speed)
                newSpeed = -boid.speed * dot;

            boid.AddForwardAcceleration(newSpeed);
        }
        #endregion

    }

}
