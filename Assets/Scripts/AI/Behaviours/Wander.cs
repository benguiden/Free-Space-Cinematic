using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace{

    [AddComponentMenu ("Boid Behaviours/Wander")]
    [RequireComponent (typeof (BoidActor))]
    public class Wander : BoidBehaviour {

        #region Public Variables
        public WanderType wanderType;

        [Header ("Movement")]
        public float cruiseSpeed = 20f;
        public float distance = 50f;
        public float radius = 25f;
        public float frequency = 0.25f;
        public float amplitude = 30f;
        public Axis axis = Axis.Horizontal;
        #endregion

        #region Private Variables
        private Vector3 desiredPosition = Vector3.zero;
        private Vector3 target = Vector3.zero;
        private float theta;
        private float seed;
        #endregion

        #region Mono Methods
        private void Start() {
            theta = Random.Range (0f, 2f * Mathf.PI);
            seed = Random.Range (-100f, 100f);
        }

        private void OnDrawGizmos() {
            if ((Application.isPlaying) && (enabled)) {
                Vector3 spherePosition = transform.TransformPoint (Vector3.forward * distance);
                Gizmos.color = new Color (1f, 0f, 1f, 0.5f);
                Gizmos.DrawLine (transform.position, desiredPosition);
                Gizmos.DrawWireSphere (spherePosition, radius);
                Gizmos.DrawWireSphere (desiredPosition, 1f);
            }
        }
        #endregion

        #region Wander Methods
        public override Vector3 UpdateForce() {
            return boid.SeekForce (desiredPosition, cruiseSpeed);
        }

        protected override void Calculate() {
            if (wanderType == WanderType.Jitter) {
                desiredPosition = CalculateJitter ();
            } else {
                float angle;
                if (wanderType == WanderType.Harmonic) {
                    angle = CalculateHarmonicAngle ();
                } else {
                    angle = CalculateNoiseAngle ();
                }
                desiredPosition = CalculateWithAngle (angle);
            }
            theta += Time.deltaTime * Mathf.PI * 2.0f * frequency;
        }

        private Vector3 CalculateJitter() {
            Vector3 newDesiredPosition = desiredPosition;
            float jitterTimeSlice = frequency * Time.deltaTime * 100f;

            Vector3 offset = Random.insideUnitSphere * jitterTimeSlice;
            target += offset;
            target.Normalize ();
            target *= radius;

            Vector3 localtarget = target + Vector3.forward * distance;
            newDesiredPosition = transform.TransformPoint (localtarget);

            return newDesiredPosition - transform.position;
        }

        private Vector3 CalculateWithAngle(float angle) {
            Vector3 newDesiredPosition = desiredPosition;
            Vector3 yawRoll = transform.rotation.eulerAngles;
            yawRoll.x = 0;

            if (axis == Axis.Horizontal) {
                target.x = Mathf.Sin (angle);
                target.z = Mathf.Cos (angle);
                target.y = 0;
                yawRoll.z = 0;
            } else {
                target.y = Mathf.Sin (angle);
                target.z = Mathf.Cos (angle);
                target.x = 0;
            }

            target *= radius;

            Vector3 localTarget = target + (Vector3.forward * distance);

            newDesiredPosition = transform.position + Quaternion.Euler (yawRoll) * localTarget;

            return newDesiredPosition;
        }

        private float CalculateHarmonicAngle() {
            return Mathf.Sin (theta) * amplitude * Mathf.Deg2Rad;
        }

        private float CalculateNoiseAngle() {
            float noise = (Mathf.PerlinNoise (theta, seed) * 2f) - 1f; //Puts Noise in the range of (-1, 1)
            return Mathf.Sin (noise) * amplitude * Mathf.Deg2Rad;
        }
        #endregion

        #region Enums
        public enum WanderType { Jitter, Harmonic, Noise};
        public enum Axis { Horizontal, Vertical };
        #endregion

    }       

}
 