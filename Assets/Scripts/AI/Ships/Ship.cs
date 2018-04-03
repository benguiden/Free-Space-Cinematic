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

        [Header ("References")]
        public ShipCollider shipCollider, shieldCollider;
        public GameObject mainMesh;
        public GameObject[] debrisObjects;
        #endregion

        #region Hidden Variables
        //References
        [HideInInspector]
        public BoidActor boid;
        #endregion

        #region Private Variables
        #endregion

        #region Mono Methods
        private void Awake() {
            if (shipCollider != null)
                shipCollider.Initalise (this);
            if (shieldCollider != null)
                shieldCollider.Initalise (this);

            boid = GetComponent<BoidActor> ();
        }
        #endregion

        #region Battle Interaction Methods
        public void Damage(float damageInflicted) {
            Debug.Log ("Damage Dealt to " + gameObject.name);
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
            Camera.main.transform.SetParent (null);

            if (mainMesh != null)
                mainMesh.SetActive (false);

            foreach(GameObject debris in debrisObjects) {
                debris.SetActive (true);
                debris.AddComponent<Rigidbody> ().AddExplosionForce (1000f, debris.transform.position, 20f);
            }
        }
        #endregion

        #region Enums
        public enum Faction{Terrans /*Good Guys*/, Vasudans /*Bad Guys*/}
        #endregion

    }

}