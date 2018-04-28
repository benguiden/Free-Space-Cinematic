using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    [AddComponentMenu("Boid Behaviours/Flee")]
    [RequireComponent(typeof(BoidActor))]
    public class Flee : BoidBehaviour {

        #region Public Variables
        [Header("Seeking")]
        public List<Transform> avoidingBoids;

        [Header("Movement")]
        public float fleeSpeed = 20f;
        #endregion

        #region Private Variables
        private Vector3 desiredPosition;
        #endregion

        #region Mono Methods
        protected override void Awake() {
            base.Awake();
        }

        private void OnDrawGizmos() {
            if ((Application.isPlaying) && (enabled)) {
                if (boid == null)
                    boid = GetComponent<BoidActor>();

                Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
                Gizmos.DrawWireSphere(desiredPosition, 2.5f);
            }
        }
        #endregion

        #region Seek Methods
        public override Vector3 UpdateForce() {
            if (avoidingBoids != null)
                return boid.SeekForce(desiredPosition, fleeSpeed);
            else
                return Vector3.zero;
        }

        protected override void Calculate() {
            if (avoidingBoids != null) {
                desiredPosition = Vector3.zero;
                for (int i=0; i<avoidingBoids.Count; i++) {
                    if (avoidingBoids[i] != null) {
                        Vector3 difference = avoidingBoids[i].position - transform.position;
                        desiredPosition += difference.normalized * (difference.magnitude / 200f);
                    } else {

                    }
                }
                desiredPosition = transform.position - desiredPosition;
            }
                
        }
        #endregion

    }

}