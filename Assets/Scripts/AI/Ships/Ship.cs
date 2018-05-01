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
        public Object shieldDamageVFXPrefab, hullDamageVFXPrefab;
        public MeshRenderer shieldRenderer;
        public float shieldAlpha = 0.5f;
        public float shieldFlashTime = 0.5f;

        [Header("Audio")]
        public AudioSource engineAudioSource;
        public float engineBaseVolume = 0.25f;
        public float engineBasePitch = 1f;
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

        //Enemies
        [HideInInspector]
        public Ship pursuing;
        [HideInInspector]
        public int pursuers;
        #endregion

        #region Private Variables
        protected uint shipID;
        private Coroutine shieldFlashCo;
        
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

            if (engineAudioSource != null) {
                if (engineAudioSource.clip != null)
                    engineAudioSource.time = Random.Range (0f, engineAudioSource.clip.length);
            }
        }

        private void Start() {
            if (ShipManager.main.emporer != this)
                shipID = ShipManager.main.AddShip(this);

            if (shieldRenderer != null)
                shieldRenderer.enabled = true;
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
            if (shieldHealth > 0f) {
                shieldHealth -= damageInflicted;
                FlashShield ();
                if (shieldHealth <= 0f) {
                    shieldHealth = 0f;
                    shieldCollider.enabled = false;
                    shieldCollider.gameObject.SetActive (false);
                }
            } else {
                hullHealth -= damageInflicted;
                if (hullHealth <= 0f) {
                    hullHealth = 0f;
                    Kill ();
                }
            }
        }

        public Object GetVFXDamagePrefab() {
            if (shieldHealth > 0f) {
                return shieldDamageVFXPrefab;
            } else {
                return hullDamageVFXPrefab;
            }
        }

        protected void Kill() {
            foreach(BoidBehaviour behaviour in boid.behaviours) {
                behaviour.enabled = false;
            }
            boid.velocity = Vector3.zero;

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

            DestroyShip ();
        }

        protected void DestroyShip() {
            ShipManager.main.ships.Remove (shipID);
            if (pursuing != null)
                pursuing.pursuers--;

            if (VRController.main != null) {
                if (VRController.main.transform.parent = transform)
                    VRController.main.transform.parent = null;
            }

            if (this == ShipManager.main.emporer) {
                ShipManager.main.ExplodeEmporer ();
                foreach (KeyValuePair<uint, Ship> otherShip in ShipManager.main.ships) {
                    otherShip.Value.gameObject.SetActive (false);
                }
                gameObject.SetActive (false);
            } else {
                Destroy (gameObject);
            }
        }
        #endregion

        #region Audio Methods
        private void UpdateEngineAudio() {
            if (boid.maxSpeed > 0f) {
                float engineAmount = boid.speed / boid.maxSpeed;
                engineAudioSource.volume = engineVolume.Evaluate (engineAmount) * engineBaseVolume;
                engineAudioSource.pitch = enginePitch.Evaluate (engineAmount) * engineBasePitch;
            }

            if (engineAudioSource.isPlaying) {
                if (boid.speed <= 0.01f)
                    engineAudioSource.Pause ();
            } else {
                if (boid.speed > 0.01f)
                    engineAudioSource.Play ();
            }
        }
        #endregion

        #region Shield Methods
        private void FlashShield() {
            if (shieldRenderer != null) {
                if (shieldFlashCo != null)
                    StopCoroutine (shieldFlashCo);

                shieldFlashCo = StartCoroutine (IShieldFlash ());
            }
        }

        private IEnumerator IShieldFlash() {
            if (shieldRenderer != null) {
                shieldRenderer.enabled = true;

                float timeLeft = shieldFlashTime;

                while (timeLeft > 0f) {
                    Color shieldColour = shieldRenderer.material.color;
                    shieldColour.a = shieldAlpha * (timeLeft / shieldFlashTime);
                    shieldRenderer.material.color = shieldColour;
                    timeLeft -= Time.deltaTime;
                    yield return null;
                }

                shieldRenderer.enabled = false;

            }

            shieldFlashCo = null;
        }
        #endregion

        #region Enums
        public enum Faction{Terrans /*Good Guys*/, Vasudans /*Bad Guys*/}
        #endregion

    }

}