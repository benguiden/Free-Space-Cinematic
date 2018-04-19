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

        [HideInInspector]
        public Ship ship;

        [HideInInspector]
        public bool canShoot = true;
        #endregion

        #region Public Methods
        public void AttemptShoot() {
            if ((canShoot) && (enabled)) {
                Shoot ();
                canShoot = false;
                ship.StartCoroutine (IReload ());
            }
        }

        public virtual void Shoot() {
            if (enabled) {

                foreach (Transform gunPoint in gunPoints) {
                    Transform projectileTransform = ((GameObject)Object.Instantiate (projectile)).transform;

                    projectileTransform.forward = gunPoint.forward;
                    projectileTransform.position = gunPoint.position + (projectileTransform.forward * projectileSpeed * Time.deltaTime);

                    Projectile newProjectile = projectileTransform.GetComponent<Projectile> ();
                    newProjectile.speed = projectileSpeed;
                    newProjectile.damage = damage;
                    newProjectile.sourceShip = ship;
                }
            }
        }
        #endregion

        protected IEnumerator IReload() {
            canShoot = false;
            yield return new WaitForSeconds (reloadTime);
            canShoot = true;
        }
    }

}