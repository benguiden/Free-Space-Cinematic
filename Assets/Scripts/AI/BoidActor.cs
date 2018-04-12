using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace{

    [AddComponentMenu ("Boid Behaviours/Boid Actor")]
    [System.Serializable]
    public class BoidActor : MonoBehaviour {

        #region Public Visible Variables
        [Header ("Physics")]
        public float mass = 1f; //Find Use
        public Vector3 velocity;
        public Vector3 acceleration;
        public float maxSpeed = 30f;
        public float maxAcceleration = 10f;

        [Header ("Banking")]
        public float bankingAmount = 10f;
        public float bankingSpeed = 1f;
        public float rotationSpeed = 1f;
        #endregion

        #region Public Hidden Variables
        public float speed{
            get {
                return GetNetVelocity ().magnitude;
            }
        }

        [HideInInspector]
        public List<BoidBehaviour> behaviours = new List<BoidBehaviour> ();
        #endregion

        #region Mono Methods
        private void Awake() {
            BoidManager.Main ().boidActors.Add (this);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine (transform.position, transform.position + velocity);
        }

        private void OnValidate() {
            if (mass < float.Epsilon)
                mass = float.Epsilon;
            if (maxSpeed < 0f)
                maxSpeed = 0f;
        }

        private void Update() {
            UpdateBehaviours ();
            UpdatePhysics ();
        }
        #endregion

        #region Behaviour Methods
        private void UpdateBehaviours() {
            acceleration = Vector3.zero;

            for (int i=0; i<behaviours.Count; i++) {
                if (behaviours[i].enabled) {
                    Vector3 behaviourAcceleration = behaviours[i].UpdateForce () * behaviours[i].weight;
                    if (AccumulateAcceleration (ref acceleration, behaviourAcceleration))
                        break;
                }
            }

        }

        private bool AccumulateAcceleration(ref Vector3 totalAcceleration, Vector3 addedAcceleration) {
            float remainingAcceleration = maxAcceleration - totalAcceleration.magnitude;
            Vector3 clampedAcceleration = Vector3.ClampMagnitude (addedAcceleration, remainingAcceleration);
            totalAcceleration += clampedAcceleration;
            return (addedAcceleration.magnitude >= remainingAcceleration);
        }

        public T GetBehaviour<T>() where T : BoidBehaviour {
            for (int i = 0; i < behaviours.Count; i++) {
                if (behaviours[i].GetType() == typeof(T)) {
                    return (T)behaviours[i];
                }
            }
            return null;
        }
        #endregion

        #region Physics Methods
        public Vector3 GetNetVelocity() {
            return velocity + Vector3.ClampMagnitude (acceleration, maxAcceleration);
        }

        public Vector3 SeekForce(Vector3 targetPosition, float speed) {
            Vector3 difference = targetPosition - transform.position;
            difference.Normalize ();
            Vector3 desiredVelocity = difference * speed;
            return desiredVelocity - velocity;
        }

        public float GetArriveSpeed(Vector3 targetPosition, float desiredDistance, float desiredSpeed, float catchUpSpeed,bool slowDownToDistance) {
            /* This method returns the desired speed of a boid, by slowing down the boid gradually when the boid is within 1 & 1/5th of the way there.
             * This also allows for the boid to arrive at a desired speed, for instance a pursue behaviour target's speed.
             * It also allows for the boid to slow down when too close to the target so it can get back to it's desired distance, again good for the pursue behaviour.
             * But if slowDownToDistance is false, well then the boid will just stay at it's desired speed, for instance having a desired speed of zero and stopping a bit away from the target for the arrive behaviour.
             */

            float distanceToTarget = Vector3.Distance (transform.position, targetPosition);
            float arriveRadius = ((speed - desiredSpeed) * (speed - desiredSpeed)) / maxAcceleration; //This compensates the distance it will take for the boid to slow down or speed up to it's desired distance so it tends not to overshoot

            float calculatedDistance = desiredDistance + arriveRadius;

            float newSpeed = Mathf.Max (catchUpSpeed, desiredSpeed);

            if (distanceToTarget < calculatedDistance * 1.2f) {
                //Slowing down to desired speed when close to desired distance
                if (distanceToTarget >= calculatedDistance) {
                    float sigmoid = (distanceToTarget - calculatedDistance) / (calculatedDistance * 0.2f);
                    newSpeed = desiredSpeed + ((maxSpeed - desiredSpeed) * sigmoid);
                } else {
                    if (slowDownToDistance) //Slowing down below desired speed when to close to target and ahead of desired distance
                        newSpeed = desiredSpeed * (distanceToTarget / calculatedDistance);
                    else
                        newSpeed = desiredSpeed;
                }
            }

            return newSpeed;
        }

        private void UpdatePhysics() {
            velocity += Vector3.ClampMagnitude (acceleration, maxAcceleration) * Time.deltaTime;
            velocity = Vector3.ClampMagnitude (velocity, maxSpeed);

            Vector3 newUp = Bank ();
            if (speed > 0.1f)
                transform.LookAt (transform.position + LerpForward().normalized, newUp);
            
            velocity *= (1.0f - (0.1f/*damping*/ * Time.deltaTime));

            transform.position += velocity * Time.deltaTime;
        }

        private Vector3 Bank() {
            Vector3 globalUp = new Vector3 (0f, 0.2f, 0f);
            Vector3 accelUp = acceleration * bankingAmount * 0.01f;
            Vector3 bankUp = accelUp + globalUp;
            Vector3 tempUp = transform.up;
            tempUp = Vector3.Lerp (tempUp, bankUp, Time.deltaTime * bankingSpeed);
            return tempUp;
        }

        private Vector3 LerpForward() {
            if (velocity.magnitude > 0.1f) {
                Quaternion currentRotation = Quaternion.LookRotation (transform.forward);
                Quaternion desiredRotation = Quaternion.LookRotation (velocity.normalized);
                desiredRotation = Quaternion.Lerp (currentRotation, desiredRotation, Time.deltaTime * rotationSpeed);
                return desiredRotation * Vector3.forward;
            } else {
                return transform.forward;
            }
        }
        #endregion
    }

}
