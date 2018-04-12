using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    public class Ship : MonoBehaviour{

        #region Public Variables
        [Header ("Stats")]
        public Faction faction;
        public float hullHealth = 100f;
        public float shieldHealth = 500f;

        [Header ("Visuals")]
        public Object destroyVFXPrefab;

        [Header ("References")]
        public ShipCollider shipCollider, shieldCollider;
        public GameObject mainMesh;
        public Debris[] debrisObjects;
        public ParticleSystem[] thrusters;
        #endregion

        #region Hidden Variables
        //State Machine
        [HideInInspector]
        public ShipStateMachine stateMachine;

        //References
        [HideInInspector]
        public BoidActor boid;
        #endregion

        #region Private Variables
        protected uint shipID;
        #endregion

        #region Mono Methods
        private void Awake() {
            if (shipCollider != null)
                shipCollider.Initalise (this);
            if (shieldCollider != null)
                shieldCollider.Initalise (this);

            boid = GetComponent<BoidActor> ();

            foreach (Debris debris in debrisObjects) {
                debris.gameObject.SetActive (false);
            }
        }

        private void Start() {
            if (ShipManager.main.emporer != this)
                shipID = ShipManager.main.AddShip(this);
        }
        #endregion

        #region Battle Interaction Methods
        public void Damage(float damageInflicted) {
            Debug.Log (gameObject.name + " Health: " + (shieldHealth + hullHealth).ToString() + ".\n");
            if (shieldHealth > 0f) {
                shieldHealth -= damageInflicted;
                if (shieldHealth <= 0f) {
                    shieldHealth = 0f;
                    //Deactivate Shield
                }
            } else {
                hullHealth -= damageInflicted;
                if (hullHealth <= 0f) {
                    hullHealth = 0f;
                    Kill ();
                }
            }
        }

        protected void Kill() {
            foreach(BoidBehaviour behaviour in boid.behaviours) {
                behaviour.enabled = false;
            }
            boid.velocity = Vector3.zero;

            //Camera.main.transform.SetParent (null);

            if (mainMesh != null)
                mainMesh.SetActive (false);

            foreach (Debris debris in debrisObjects) {
                debris.gameObject.SetActive (true);
                debris.transform.SetParent (null);
                debris.Activate (transform.position, 50f, 10f);
            }

            foreach (ParticleSystem thruster in thrusters) {
                thruster.gameObject.SetActive (false);
            }

            Transform destroyVFXTransform = ((GameObject)Instantiate (destroyVFXPrefab, null)).transform;
            destroyVFXTransform.position = transform.position;
            destroyVFXTransform.localEulerAngles = transform.localEulerAngles;
        }
        #endregion

        #region Enums
        public enum Faction{Terrans /*Good Guys*/, Vasudans /*Bad Guys*/}
        #endregion

    }

}