﻿using System.Collections;
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
        private Vector3 desiredPosition;
        public float desiredSpeed;
        #endregion

        #region Mono Methods
        protected override void Awake() {
            base.Awake ();
        }

        private void OnDrawGizmos() {
            if (Application.isPlaying) {
                if (boid == null)
                    boid = GetComponent<BoidActor> ();

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere (desiredPosition, 2.5f);
            }
        }

        public override Vector3 UpdateForce() {
            return boid.SeekForce (desiredPosition, desiredSpeed);
        }
        #endregion

        #region Seek Methods
        protected override void Calculate() {
            desiredSpeed = boid.GetArriveSpeed (target.transform.position, desiredDistance, target.speed, true);

            float timeToTarget = Vector3.Distance (transform.position, target.transform.position) / desiredSpeed;
            desiredPosition = target.transform.position + (timeToTarget * target.velocity);
        }
        #endregion

    }

}