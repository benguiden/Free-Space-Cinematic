﻿using System.Collections;
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


            Debug.Log(Vector3.Dot(target.position.normalized, boid.transform.position.normalized));
            float newSpeed = desiredSpeed * Vector3.Dot(target.position.normalized, boid.transform.position.normalized);
            boid.SetForwardSpeed(desiredSpeed);
        }

        private Vector3 DesiredVelocityToAcceleration() {

            return Vector3.zero;
        }
        #endregion

    }

}
