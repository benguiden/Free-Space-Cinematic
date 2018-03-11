﻿//#define MULTIPLE_BEHAVIOURS

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
        public float maxAcceleration = 10f;

        [Header ("Banking")]
        public float bankingAmount = 15f;
        public float bankingSpeed = 0.2f;

        [Header ("Behaviours")]
        public PathFollower pathFollowing;
        public Seek seek;
        

        //Excluded for the time being until more behaviours are developed
        #if MULTIPLE_BEHAVIOURS
        public BoidBehaviour[] behaviours;
        #endif
        //////////
        #endregion

        #region Public Hidden Variables
        public float speed{
            get {
                return GetNetVelocity ().magnitude;
            }
        }
        #endregion

        #region Private Variables
        //Banking
        private Vector3 lastForward;
        private Vector3 desiredForward;
        private float currentBank = 0f;
        #endregion

        #region Mono Methods
        private void Awake() {
            pathFollowing.SetBoidActor (this);
            seek.SetBoidActor(this);

            lastForward = transform.forward;
            desiredForward = transform.forward;

            AwakeBehaviours ();
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine (transform.position, transform.position + velocity);

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
            acceleration += desiredForward * forwardForce * Time.deltaTime;
        }
        public void AddForce(Vector3 addedForce) {
            acceleration += addedForce * mass * Time.deltaTime;
        }
        public void AddForwardAcceleration(float forwardAcceleration) {
            acceleration += desiredForward * forwardAcceleration * Time.deltaTime;
        }
        public void AddAcceleration(Vector3 addedAcceleration) {
            acceleration += addedAcceleration * Time.deltaTime;
        }
        public void SetForwardSpeed(float newSpeed) {
            velocity = desiredForward * newSpeed;
        }

        public Vector3 GetNetVelocity() {
            return velocity + Vector3.ClampMagnitude (acceleration, maxAcceleration);
        }
        #endregion

        #region Angular Forces
        //Has to be improved in the future
        public void SpinToTargetForward(Vector3 targetForward, float speed) {
            desiredForward = Vector3.Lerp(desiredForward, targetForward, speed);
        }
        #endregion

        private void UpdatePhysics() {
            velocity += Vector3.ClampMagnitude (acceleration, maxAcceleration);
            velocity = Vector3.ClampMagnitude (velocity, maxSpeed);

            ReduceSidewardsDrag ();
            Bank ();

            acceleration = Vector3.zero;

            transform.position += velocity * Time.deltaTime;
        }

        private void Bank() {
            Vector3 desiredForward2D = new Vector3 (desiredForward.x, 0f, desiredForward.z);
            Vector3 lastForward2D = new Vector3 (lastForward.x, 0f, lastForward.z);

            currentBank = Mathf.Lerp (currentBank,
                Vector3.SignedAngle (desiredForward2D, lastForward2D, Vector3.up) * (bankingAmount / 20f),
                Time.deltaTime * 60f * bankingSpeed);

            Vector3 newUp = new Vector3 (-currentBank, 1f, 0f).normalized;
        
            if (speed > float.Epsilon) {
                transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);
                transform.rotation = Quaternion.LookRotation (desiredForward, transform.TransformDirection (newUp));
            }

            desiredForward = transform.forward;
            lastForward = transform.forward;
        }

        private void ReduceSidewardsDrag() {
            Vector3 lateralVelocity = transform.InverseTransformDirection (velocity);
            velocity -= transform.right * lateralVelocity.x * Time.deltaTime;
            velocity -= transform.up * lateralVelocity.y * Time.deltaTime;
        }
        #endregion
    }

}
