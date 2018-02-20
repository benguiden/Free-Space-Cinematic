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

        //Excluded for the time being until more behaviours are developed
        #if MULTIPLE_BEHAVIOURS
        public List<BoidBehaviour> behaviours;
        #endif
        #endregion
        //////////

        #region Public Hidden Variables
        //[HideInInspector]
        
        #endregion

        #region Mono Methods
        private void Awake() {
            pathFollowing.SetBoidActor (this);

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
                behaviours[i].Update ();
            }
            #endif

            pathFollowing.Awake ();
        }

        private void UpdateBehaviours() {

            //Excluded for the time being until more behaviours are developed
            #if MULTIPLE_BEHAVIOURS
            for (int i=0; i<behaviours.Count; i++) {
                behaviours[i].Update ();
            }
            #endif
            //////////

            pathFollowing.Update ();

        }

        private void GizmosBehaviours() {
            #if MULTIPLE_BEHAVIOURS
            for (int i=0; i<behaviours.Count; i++) {
                behaviours[i].Update ();
            }
            #endif

            pathFollowing.OnDrawGizmos ();
        }
        #endregion

        #region Physics Methods
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

        private void UpdatePhysics() {
            velocity += acceleration;
            velocity = Vector3.ClampMagnitude (velocity, maxSpeed);
            acceleration = Vector3.zero;

            transform.position += velocity * Time.deltaTime;
        }
        #endregion
    }

}
