using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    [System.Serializable]
    public class Gun{

        #region Public Variables
        public bool enabled;
        
        public Transform[] gunPoints;
        
        [Header ("Projectiles")]
        public Object projectile;
        public float projectileSpeed = 5000f;
        public float damage = 25f;
        public float reloadTime = 1f;

        [Header ("Audio")]
        public AudioSource shootAudioSource;
        public AudioClip shootAudioClip;

        [HideInInspector]
        public Ship ship;

        [HideInInspector]
        public bool canShoot = true;
        #endregion

        #region Public Methods
        public bool AttemptShoot() {
            if ((canShoot) && (enabled)) {
                Shoot ();
                canShoot = false;
                ship.StartCoroutine (IReload ());
                return true;
            } else {
                return false;
            }
        }

        public virtual void Shoot() {
            if (enabled) {

                bool firstProjectile = true;
                foreach (Transform gunPoint in gunPoints) {
                    Transform projectileTransform = ((GameObject)Object.Instantiate (projectile)).transform;

                    projectileTransform.forward = gunPoint.forward;
                    projectileTransform.position = gunPoint.position + (projectileTransform.forward * projectileSpeed * Time.deltaTime);

                    Projectile newProjectile = projectileTransform.GetComponent<Projectile> ();
                    newProjectile.speed = projectileSpeed;
                    newProjectile.damage = damage;
                    newProjectile.sourceShip = ship;
                    newProjectile.faction = ship.faction;

                    if (firstProjectile) {
                        AudioSource projectileAudioSource = newProjectile.GetComponent<AudioSource> ();
                        if (projectileAudioSource != null) {
                            firstProjectile = false;
                            projectileAudioSource.enabled = false;
                        }
                    }
                }

                if (shootAudioSource != null) {
                    shootAudioSource.clip = shootAudioClip;
                    shootAudioSource.time = 0f;
                    shootAudioSource.Play ();
                }

            }
        }

        public bool AimingAt(BoidActor targetBoid, float accuracy) {
            float timeToTarget = Vector3.Distance (targetBoid.transform.position, ship.transform.position) / projectileSpeed;
            Vector3 targetPosition = targetBoid.transform.position + (targetBoid.velocity * timeToTarget);

            if (Vector3.Angle (ship.transform.forward, targetPosition - ship.transform.position) <= accuracy)
                return true;
            else
                return false;
        }
        #endregion

        protected IEnumerator IReload() {
            canShoot = false;
            yield return new WaitForSeconds (reloadTime);
            canShoot = true;
        }
    }

}