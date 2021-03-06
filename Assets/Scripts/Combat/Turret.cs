﻿using System.Collections;
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

        [Header ("Shooting")]
        public Ship.Faction faction = Ship.Faction.Terrans;
        public float accuracy = 0.75f;
        public float shootingRange = 600f;
        public float reloadTime = 1f;
        public float refreshTime = 0.5f;

        [Header ("Projectiles")]
        public Object projectile;
        public float projectileSpeed = 5000f;
        public float damage = 25f;
        #endregion

        #region Private Variables
        private Ship target;
        private bool canShoot = true;
        private Coroutine reloadCo;
        private Vector3 targetPosition;
        #endregion

        #region Mono Methods
        private void Start() {
            InitaliseTargetPosition ();
            StartCoroutine (CinematicAngle ());
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

                targetPosition = target.transform.position + (target.boid.velocity * timeToHit);

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

        private Ship CalculateTarget() {
            Ship newTarget = null;
            float closestDistance = shootingRange;
            foreach (Ship ship in ShipManager.main.ships.Values) { //Test which enemy boid is closest to turret
                if ((ship != ShipManager.main.emporer) && (ship.isActiveAndEnabled)) { //Change to isEnemy
                    if (ship.faction != faction) {
                        Vector3 shipPosition = ship.transform.position;

                        if ((Mathf.Abs (shipPosition.x - transform.position.x) < shootingRange)      //Calculating the distance to the boid in a box first
                            && (Mathf.Abs (shipPosition.y - transform.position.y) < shootingRange)   //As to eliminate most boids from the distance calculation
                            && (Mathf.Abs (shipPosition.z - transform.position.z) < shootingRange)) {//Which uses the very taxing Sqrt() function

                            float distanceToBoid = Vector3.Distance (transform.position, shipPosition);
                            if (distanceToBoid < closestDistance) {
                                newTarget = ship;
                                closestDistance = distanceToBoid;
                            }
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

            switch (barrelDirection) {
                case BarrelDirection.Forward:
                    projectileTransform.forward = transform.forward;
                    break;
                case BarrelDirection.nForward:
                    projectileTransform.forward = -transform.forward;
                    break;
                case BarrelDirection.Up:
                    projectileTransform.forward = transform.up;
                    break;
                case BarrelDirection.nUp:
                    projectileTransform.forward = -transform.up;
                    break;
            }

            projectileTransform.position = transform.position + (projectileTransform.forward * projectileSpeed * Time.deltaTime);

            Projectile newProjectile = projectileTransform.GetComponent<Projectile> ();
            newProjectile.speed = projectileSpeed;
            newProjectile.damage = damage;
            newProjectile.sourceShip = ShipManager.main.emporer;

            canShoot = false;
            if (reloadCo == null)
                reloadCo = StartCoroutine (IReload ());
        }

        private IEnumerator CinematicAngle() {
            yield return new WaitForSeconds (Random.Range (0f, 5f));
            while (enabled) {
                if ((Random.Range(0f, 1f) < 0.1f) && (target != null)) {
                    CameraAngle newCameraAngle = new CameraAngle ();
                    newCameraAngle.fovRange = new Vector2 (65f, 80f);
                    newCameraAngle.distanceRange = new Vector2 (35f, 65f);
                    newCameraAngle.timeRange = new Vector2 (2f, 4f);
                    newCameraAngle.interestTime = 5f;
                    newCameraAngle.interest = 1f;
                    newCameraAngle.stationary = true;
                    newCameraAngle.localOffset = true;
                    newCameraAngle.focus = transform;
                    Director.main.AddAngle (newCameraAngle);
                }
                yield return new WaitForSeconds (Random.Range (4.5f, 11.5f));
            }
        }
        #endregion

        #region Movement Methods
        private void LookAtTarget() {
            Vector3 accuracyOffset = Vector3.zero;
            float accuracyK = 20f / accuracy;
            if (accuracy > 0f)
                accuracyOffset = new Vector3 (Random.Range (-accuracyK, accuracyK), Random.Range (-accuracyK, accuracyK), Random.Range (-accuracyK, accuracyK));

            Quaternion newRotation = Quaternion.LookRotation (targetPosition - transform.position + accuracyOffset);
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
            transform.rotation =  Quaternion.Lerp (transform.rotation, newRotation, angularSpeed * Time.deltaTime * 50f);
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
