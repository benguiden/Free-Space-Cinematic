using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    public class Turret : MonoBehaviour {

        #region Public Varibles
        [Header("Movement")]
        public Vector3 angleRange;
        public BarrelDirection barrelDirection = BarrelDirection.Up;
        [Range(float.Epsilon, 1f)]
        public float angularSpeed = 0.25f;

        [Header("Shooting")]
        public float shootingRange = 600f;
        public float reloadTime = 1f;
        public float refreshTime = 0.5f;

        [Header ("Projectiles")]
        public Object projectile;
        public float projectileSpeed = 5000f;
        public float damage = 100f;
        #endregion

        #region Private Variables
        private BoidActor target;
        private bool canShoot = true;
        private Coroutine reloadCo;
        private Vector3 targetPosition;
        #endregion

        #region Mono Methods
        private void Start() {
            InitaliseTargetPosition ();
        }

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

                targetPosition = target.transform.position + (target.velocity * timeToHit);

                LookAtTarget ();

                if ((canShoot) && (!ClampRotation ()))
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

        #region Movement Methods
        private void LookAtTarget() { 
            Quaternion newRotation = Quaternion.LookRotation (targetPosition - transform.position);
            Vector3 newEulerRotation = newRotation.eulerAngles;
            switch (barrelDirection) {
                case BarrelDirection.Forward:
                    break;
                case BarrelDirection.nForward:
                    newEulerRotation = (newEulerRotation + new Vector3 (180f, 0f, 180f));
                    break;
                case BarrelDirection.Up:
                    newEulerRotation = (newEulerRotation + new Vector3 (-90f, 0f, 180f));
                    break;
                case BarrelDirection.nUp:
                    newEulerRotation = (newEulerRotation + new Vector3 (-90f, 0f, 0f));
                    break;
            }
            newRotation = Quaternion.Euler (newEulerRotation);
            transform.rotation = Quaternion.Lerp (transform.rotation, newRotation, angularSpeed * Time.deltaTime * 10f);
        }

        private bool ClampRotation() { //Returns true if the turret can look at the taret within it's rotational clamps
            bool hadToClamp = false;

            Vector3 clampedEulerAngles = transform.localEulerAngles;

            //X
            while (clampedEulerAngles.x < 0f)
                clampedEulerAngles.x += 360f; //Force angle to be positive so the Mod function works correctly
            clampedEulerAngles.x = clampedEulerAngles.x % 360f; //Put angle in the range of (0, 360)

            //Y
            while (clampedEulerAngles.y < 0f)
                clampedEulerAngles.y += 360f;
            clampedEulerAngles.y = clampedEulerAngles.y % 360f;

            //Z
            while (clampedEulerAngles.z < 0f)
                clampedEulerAngles.z += 360f;
            clampedEulerAngles.z = clampedEulerAngles.z % 360f;

            //X
            if ((clampedEulerAngles.x < 180f) && (clampedEulerAngles.x > angleRange.x)) {
                clampedEulerAngles.x = angleRange.x;
                hadToClamp = true;
            } else if ((clampedEulerAngles.x > 180f) && (clampedEulerAngles.x < 360f - angleRange.x)) {
                clampedEulerAngles.x = 360f - angleRange.x;
                hadToClamp = true;
            }

            //Y
            if ((clampedEulerAngles.y < 180f) && (clampedEulerAngles.y > angleRange.y)) {
                clampedEulerAngles.y = angleRange.y;
                hadToClamp = true;
            } else if ((clampedEulerAngles.y > 180f) && (clampedEulerAngles.y < 360f - angleRange.y)) {
                clampedEulerAngles.y = 360f - angleRange.y;
                hadToClamp = true;
            }

            //Z
            if ((clampedEulerAngles.z < 180f) && (clampedEulerAngles.z > angleRange.z)) {
                clampedEulerAngles.z = angleRange.z;
                hadToClamp = true;
            } else if ((clampedEulerAngles.z > 180f) && (clampedEulerAngles.z < 360f - angleRange.z)) {
                clampedEulerAngles.z = 360f - angleRange.z;
                hadToClamp = true;
            }

            transform.localEulerAngles = clampedEulerAngles;

            return hadToClamp;

        }

        private void InitaliseTargetPosition() {
            switch (barrelDirection) {
                case BarrelDirection.Forward:
                    targetPosition = transform.position + transform.forward;
                    break;
                case BarrelDirection.nForward:
                    targetPosition = transform.position - transform.forward;
                    break;
                case BarrelDirection.Up:
                    targetPosition = transform.position + transform.up;
                    break;
                case BarrelDirection.nUp:
                    targetPosition = transform.position - transform.up;
                    break;
            }
        }
        #endregion

        public enum BarrelDirection { Forward, nForward, Up, nUp}

    }

}
