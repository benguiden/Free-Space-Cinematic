using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    [System.Serializable]
    public class Seek : BoidBehaviour {

        #region Public Variables
        [Header("Seeking")]
        public Transform target;

        [Header("Movement")]
        public float desiredSpeed;
        #endregion

        #region Private Variables

        #endregion

        #region Constructors
        public Seek(BoidActor boidActor) : base(boidActor) {

        }
        #endregion

        #region Boid Methods
        public override void Update() {
            MoveTowardTarget();
        }
        #endregion

        #region Seek Methods
        
        private void MoveTowardTarget() {
            Vector3 desiredForward = (target.position - boid.transform.position).normalized;
            boid.SpinToTargetForward(desiredForward, 0.05f);

            
            float dot = Vector3.Dot(boid.transform.forward, desiredForward);
            float newSpeed = desiredSpeed * dot;
            boid.AddForwardAcceleration(newSpeed);
        }

        private Vector3 DesiredVelocityToAcceleration() {

            return Vector3.zero;
        }
        #endregion

    }

}
