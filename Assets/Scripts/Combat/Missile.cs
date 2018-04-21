using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    public class Missile : Projectile {

        private ParticleSystem particleTrail;

        protected override void Awake() {
            base.Awake();

            particleTrail = GetComponentInChildren<ParticleSystem>();

            if ((particleTrail != null) && (!enabled)) {
                particleTrail.gameObject.SetActive(false);
            }
        }

        protected override void Update() {
            if (lifetime > 0f) {
                lifetime -= Time.deltaTime;
                transform.position += -transform.up * speed * Time.deltaTime;
            } else {
                Destroy(gameObject);
            }
        }

        protected override void OnEnable() {
            base.OnEnable();
            particleTrail.gameObject.SetActive(true);
        }

    }

}