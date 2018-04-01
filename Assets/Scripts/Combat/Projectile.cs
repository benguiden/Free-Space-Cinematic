using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    public class Projectile : MonoBehaviour{

        #region Public Variables
        [Header ("Movement")]
        public float speed = 5000f;
        public float lengthK = 5f;
        public float sizeK = 2f;
        public float lifetime = 1f;
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
        #endregion

    }
}
