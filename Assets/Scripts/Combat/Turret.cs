using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    public class Turret : MonoBehaviour{

        #region Public Varibles
        public Object projectile;
        public float projectileSpeed = 5000f;
        public Vector3 angleRange;
        public float shootingRange = 600f;
        public float reloadTime = 1f;
        public float refreshTime = 0.5f;
        #endregion

        #region Private Variables
        private BoidActor target;
        private bool canShoot = true;
        private Coroutine reloadCo;
        #endregion

        #region Mono Methods
        private void OnDrawGizmosSelected() {
            if (enabled) {
                Gizmos.color = new Color (1f, 0f, 0f, 0.5f);
                Gizmos.DrawWireSphere (transform.position, shootingRange);
            }
        }

        private void OnEnable() {
            StartCoroutine (ITargetBoid ());
        }

        private void Update() {
            if (target != null) {
                float timeToHit = Vector3.Distance (transform.position, target.transform.position) / projectileSpeed;
                Vector3 targetPosition = target.transform.position + (target.velocity * timeToHit);

                transform.up = targetPosition - transform.position;

                if (canShoot)
                    Shoot ();
            }
        }
        #endregion

        #region Calculate Methods
        private IEnumerator ITargetBoid() {
            yield return new WaitForSeconds (Random.Range (0f, refreshTime));
            while (enabled) {
                if (target == null)
                    target = CalculateTarget ();
                else {
                    if (Vector3.Distance (transform.position, target.transform.position) > shootingRange)
                        target = CalculateTarget ();
                }

                yield return new WaitForSeconds (refreshTime);
            }
        }

        private BoidActor CalculateTarget() {
            BoidActor newTarget = null;
            float closestDistance = shootingRange;
            foreach (BoidActor boidActor in BoidManager.Main ().boidActors) { //Test which enemy boid is closest to turret
                if (true) { //Change to isEnemy
                    Vector3 boidPosition = boidActor.transform.position;

                    if ((Mathf.Abs (boidPosition.x - transform.position.x) < shootingRange)      //Calculating the distance to the boid in a box first
                        && (Mathf.Abs (boidPosition.y - transform.position.y) < shootingRange)   //As to eliminate most boids from the distance calculation
                        && (Mathf.Abs (boidPosition.z - transform.position.z) < shootingRange)) {//Which uses the very taxing Sqrt() function

                        float distanceToBoid = Vector3.Distance (transform.position, boidPosition);
                        if (distanceToBoid < closestDistance) {
                            newTarget = boidActor;
                            closestDistance = distanceToBoid;
                        }
                    }
                }
            }
            return newTarget;
        }
        #endregion

        #region Attack Methods
        private IEnumerator IReload() {
            yield return new WaitForSeconds (reloadTime);
            canShoot = true;
            reloadCo = null;
        }

        private void Shoot() {
            Transform projectileTransform = ((GameObject)Instantiate (projectile)).transform;
            projectileTransform.position = transform.position;
            projectileTransform.forward = transform.up;
            canShoot = false;
            if (reloadCo == null)
                reloadCo = StartCoroutine (IReload ());
        }
        #endregion

    }

}
