using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    [System.Serializable]
    public class Seek : BoidBehaviour {

        #region Public Variables
        [Header("Seeking")]
        public Transform target;

        [Header ("Movement")]
        public float cruiseSpeed = 5f;
        public float desiredArriveSpeed = 5f;
        public float nearingDistance = 2f;
        #endregion

        #region Private Variables

        #endregion

        #region Constructors
        public Seek(BoidActor boidActor) : base(boidActor) {

        }
        #endregion

        #region Boid Methods
        public override void OnDrawGizmos() {
            if (Application.isPlaying) {
                float timeToDesired = (boid.speed - desiredArriveSpeed) / boid.maxAcceleration;
                float radius = timeToDesired * boid.speed;
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere (target.position, radius);
            }
        }

        public override void Update() {
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
