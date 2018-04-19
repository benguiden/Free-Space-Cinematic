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

        [HideInInspector]
        public Ship sourceShip;
        #endregion

        #region Private Variables
        protected bool canCauseDamage = true;
        protected AudioSource audioSource;
        #endregion

        #region Mono Methods
        protected virtual void Awake() {
            audioSource = GetComponent<AudioSource> ();
            
            if ((audioSource != null) && (!enabled)) {
                audioSource.Stop();
                audioSource.enabled = false;
            }
        }

        protected virtual void Start() {
            if (audioSource != null)
                audioSource.pitch += Random.Range (-0.05f, 0.05f);
        }

        protected virtual void Update() {
            if (lifetime > 0f) {
                lifetime -= Time.deltaTime;
                transform.position += transform.forward * speed * Time.deltaTime;
                transform.localScale = new Vector3 (sizeK, sizeK, (speed * Time.deltaTime) / lengthK);
            } else {
                Destroy (gameObject);
            }
        }

        protected virtual void OnTriggerEnter(Collider other) {
            if ((canCauseDamage) && (isActiveAndEnabled)) {
                if (other.gameObject.tag == "ShipCollider") {
                    ShipCollider shipCollider = other.GetComponent<ShipCollider> ();
                    if (shipCollider.ship != sourceShip) {
                        shipCollider.ship.Damage (damage);
                        canCauseDamage = false;
                        Destroy ();
                    }
                }
            }
        }

        protected virtual void OnEnable() {
            audioSource.enabled = true;
            audioSource.time = 0f;
            audioSource.Play();
        }
        #endregion

        #region Interaction Methods
        protected virtual void Destroy() {
           // Destroy (gameObject);
        }
        #endregion

    }
}
