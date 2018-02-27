//#define MULTIPLE_BEHAVIOURS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace FreeSpace{

    public class BoidActor : MonoBehaviour {

        #region Public Visible Variables
        [Header ("Physics")]
        public float mass = 1f;
        public Vector3 velocity;
        public Vector3 acceleration;
        public float maxSpeed = 10f;

        [Header ("Behaviours")]
        public PathFollower pathFollowing;
        public Seek seek;

        //Excluded for the time being until more behaviours are developed
        #if MULTIPLE_BEHAVIOURS
        public BoidBehaviour[] behaviours;
        #endif
        #endregion
        //////////

        #region Public Hidden Variables
        //[HideInInspector]
        
        #endregion

        #region Mono Methods
        private void Awake() {
            pathFollowing.SetBoidActor (this);
            seek.SetBoidActor(this);

            AwakeBehaviours ();
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine (transform.position, transform.position + velocity * 2f);

            GizmosBehaviours ();

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
        private void AwakeBehaviours() {

            #if MULTIPLE_BEHAVIOURS
            for (int i=0; i<behaviours.Count; i++) {
            if (behaviours[i].enabled)
                behaviours[i].Awake ();
            }
            #endif

            if (pathFollowing.enabled)
                pathFollowing.Awake();

            if (seek.enabled)
                seek.Awake();
        }

        private void UpdateBehaviours() {

            //Excluded for the time being until more behaviours are developed
            #if MULTIPLE_BEHAVIOURS
            for (int i=0; i<behaviours.Count; i++) {
                if (behaviours[i].enabled)
                    behaviours[i].Update ();
            }
            #endif
            //////////

            if (pathFollowing.enabled)
                pathFollowing.Update();

            if (seek.enabled)
                seek.Update();

        }

        private void GizmosBehaviours() {
            #if MULTIPLE_BEHAVIOURS
            for (int i=0; i<behaviours.Count; i++) {
                if (behaviours[i].enabled)
                    behaviours[i].OnDrawGizmos ();
            }
            #endif

            if (pathFollowing.enabled)
                pathFollowing.OnDrawGizmos();

            if (seek.enabled)
                seek.OnDrawGizmos();
        }
        #endregion

        #region Physics Methods
        #region Displacement Forces
        public void AddForwardForce(float forwardForce) {
            acceleration += transform.forward * forwardForce * Time.deltaTime;
        }
        public void AddForce(Vector3 addedForce) {
            acceleration += addedForce * mass * Time.deltaTime;
        }
        public void AddForwardAcceleration(float forwardAcceleration) {
            acceleration += transform.forward * forwardAcceleration * Time.deltaTime;
        }
        public void AddAcceleration(Vector3 addedAcceleration) {
            acceleration += addedAcceleration * Time.deltaTime;
        }
        public void SetForwardSpeed(float newSpeed) {
            velocity = transform.forward * newSpeed;
        }
        #endregion

        #region Angular Forces
        //Has to be improved in the future
        public void SpinToTargetForward(Vector3 targetForward, float speed) {
            transform.forward = Vector3.Lerp(transform.forward, targetForward, speed);
        }
        #endregion

        private void UpdatePhysics() {
            velocity += acceleration;
            velocity = Vector3.ClampMagnitude (velocity, maxSpeed);
            acceleration = Vector3.zero;

            transform.position += velocity * Time.deltaTime;
        }
        #endregion
    }

}
