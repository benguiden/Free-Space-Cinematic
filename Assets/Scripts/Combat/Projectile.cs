using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{
    [RequireComponent(typeof(Collider))]
    public class Projectile : MonoBehaviour{

        #region Public Variables
        [Header ("Stats")]
        public float damage = 25f;

        [Header ("Movement")]
        public float speed = 5000f;
        public float lengthK = 5f;
        public float sizeK = 2f;
        public float lifetime = 1f;
        #endregion

        #region Private Variables
        private bool canCauseDamage = true;
        #endregion

        #region Mono Methods
        private void Update() {
            if (lifetime > 0f) {
                lifetime -= Time.deltaTime;
                transform.position += transform.forward * speed * Time.deltaTime;
                transform.localScale = new Vector3 (sizeK, sizeK, (speed * Time.deltaTime) / lengthK);
            } else {
                Destroy (gameObject);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (canCauseDamage) {
                if (other.gameObject.tag == "ShipCollider") {
                    other.GetComponent<ShipCollider> ().ship.Damage (damage);
                    canCauseDamage = false;
                    Destroy ();
                }
            }
        }
        #endregion

        #region Interaction Methods
        private void Destroy() {
            Destroy (gameObject);
        }
        #endregion

    }
}
