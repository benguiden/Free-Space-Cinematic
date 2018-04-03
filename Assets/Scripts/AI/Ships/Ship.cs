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
        #endregion

        #region Hidden Variables
        #endregion

        #region Private Variables
        #endregion

        #region Mono Methods
        private void Awake() {
            if (shipCollider != null)
                shipCollider.Initalise (this);
            if (shieldCollider != null)
                shieldCollider.Initalise (this);
        }
        #endregion

        #region Battle Interaction Methods
        public void Damage(float damage) {
            Debug.Log ("Damage Dealt to " + gameObject.name);
            if (shieldHealth > 0f) {
                shieldHealth -= damage;
                if (shieldHealth <= 0f) {
                    shieldHealth = 0f;
                    //Deactivate Shield
                }
            } else {
                hullHealth -= damage;
                if (hullHealth <= 0f) {
                    hullHealth = 0f;
                    Kill ();
                }
            }
        }

        protected void Kill() {
            gameObject.SetActive (false);
        }
        #endregion

        #region Enums
        public enum Faction{Terrans /*Good Guys*/, Vasudans /*Bad Guys*/}
        #endregion

    }

}