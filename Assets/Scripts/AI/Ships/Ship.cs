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

        [Header ("Visuals")]
        public Object destroyVFXPrefab;

        [Header("Audio")]
        public AudioSource engineAudioSource;
        [Tooltip ("Put in the range (0,1)")]
        public AnimationCurve engineVolume, enginePitch;

        [Space]
        public Gun[] guns;

        [Header ("References")]
        public ShipCollider shipCollider;
        public ShipCollider shieldCollider;
        public GameObject mainMesh;
        public Debris[] debrisObjects;
        public ParticleSystem[] thrusters;
        #endregion

        #region Hidden Variables
        //State Machine
        [HideInInspector]
        public ShipStateMachine stateMachine;

        //References
        [HideInInspector]
        public BoidActor boid;
        #endregion

        #region Private Variables
        protected uint shipID;
        #endregion

        #region Mono Methods
        protected virtual void Awake() {
            if (shipCollider != null)
                shipCollider.Initalise (this);
            if (shieldCollider != null)
                shieldCollider.Initalise (this);

            boid = GetComponent<BoidActor> ();

            foreach (Debris debris in debrisObjects) {
                debris.gameObject.SetActive (false);
            }

            foreach (Gun gun in guns) {
                gun.ship = this;
            }
        }

        private void Start() {
            if (ShipManager.main.emporer != this)
                shipID = ShipManager.main.AddShip(this);
        }

        private void Update() {
            if (engineAudioSource != null)
                UpdateEngineAudio ();
        }

        private void OnEnable() {
            foreach (Gun gun in guns) {
                gun.canShoot = this;
            }
        }
        #endregion

        #region Battle Interaction Methods
        public void Damage(float damageInflicted) {
            Debug.Log (gameObject.name + " Health: " + (shieldHealth + hullHealth).ToString() + ".\n");
            if (shieldHealth > 0f) {
                shieldHealth -= damageInflicted;
                if (shieldHealth <= 0f) {
                    shieldHealth = 0f;
                    //Deactivate Shield
                }
            } else {
                hullHealth -= damageInflicted;
                if (hullHealth <= 0f) {
                    hullHealth = 0f;
                    Kill ();
                }
            }
        }

        protected void Kill() {
            foreach(BoidBehaviour behaviour in boid.behaviours) {
                behaviour.enabled = false;
            }
            boid.velocity = Vector3.zero;

            //Camera.main.transform.SetParent (null);

            if (mainMesh != null)
                mainMesh.SetActive (false);

            foreach (Debris debris in debrisObjects) {
                debris.gameObject.SetActive (true);
                debris.transform.SetParent (null);
                debris.Activate (transform.position, 50f, 10f);
            }

            foreach (ParticleSystem thruster in thrusters) {
                thruster.gameObject.SetActive (false);
            }

            Transform destroyVFXTransform = ((GameObject)Instantiate (destroyVFXPrefab, null)).transform;
            destroyVFXTransform.position = transform.position;
            destroyVFXTransform.localEulerAngles = transform.localEulerAngles;
        }
        #endregion

        #region Audio Methods
        private void UpdateEngineAudio() {
            float engineAmount = boid.speed / boid.maxSpeed;
            engineAudioSource.volume = engineVolume.Evaluate (engineAmount);
            engineAudioSource.pitch = enginePitch.Evaluate (engineAmount);

            if (engineAudioSource.isPlaying) {
                if (boid.speed <= 0.01f)
                    engineAudioSource.Pause ();
            } else {
                if (boid.speed > 0.01f)
                    engineAudioSource.Play ();
            }
        }
        #endregion

        #region Enums
        public enum Faction{Terrans /*Good Guys*/, Vasudans /*Bad Guys*/}
        #endregion

    }

}