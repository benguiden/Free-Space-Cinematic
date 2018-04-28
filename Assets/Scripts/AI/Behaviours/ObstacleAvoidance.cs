using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    [AddComponentMenu ("Boid Behaviours/Obstacle Avoidance")]
    [RequireComponent (typeof (BoidActor))]
    public class ObstacleAvoidance : BoidBehaviour {

        #region Public Variables
        [Header("Obstacle Avoidance")]
        public float scale = 4.0f;
        public float forwardFeelerDepth = 30;
        public float sideFeelerDepth = 15;

        public float frontFeelerUpdatesPerSecond = 10.0f;
        public float sideFeelerUpdatesPerSecond = 5.0f;

        [Header("Feelers")]
        public float feelerRadius = 2.0f;
        
        public ForceType forceType = ForceType.normal;

        public LayerMask mask = -1;
        #endregion

        #region Private Variables
        private FeelerInfo[] feelers = new FeelerInfo[5];
        private Vector3 force = Vector3.zero;
        private Ship ship;
        #endregion

        #region Mono Methods
        protected override void Awake() {
            base.Awake ();
            ship = GetComponent<Ship> ();
        }

        public void OnEnable() {
            StartCoroutine (UpdateFrontFeelers ());
            StartCoroutine (UpdateSideFeelers ());
        }

        public void OnDrawGizmos() {
            if ((isActiveAndEnabled) && (Application.isPlaying)) {
                foreach (FeelerInfo feeler in feelers) {
                    Gizmos.color = Color.gray;
                    if (Application.isPlaying) {
                        Gizmos.DrawLine (transform.position, feeler.point);
                    }
                    if (feeler.collided) {
                        float depth = forwardFeelerDepth + ((boid.velocity.magnitude / boid.maxSpeed) * forwardFeelerDepth);
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine (feeler.point, feeler.point + (feeler.normal * depth));
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine (feeler.point, feeler.point + force);
                    }
                }
            }
        }
        #endregion

        #region Seek Methods
        public override Vector3 UpdateForce() {
            force = Vector3.zero;

            for (int i = 0; i < feelers.Length; i++) {
                FeelerInfo info = feelers[i];
                if (info.collided) {
                    force += CalculateSceneAvoidanceForce (info);
                }
            }
            return force;
        }

        protected override void Calculate() {
            
        }
        #endregion

        #region Avoid Methods
        private Vector3 CalculateSceneAvoidanceForce(FeelerInfo info) {
            Vector3 force = Vector3.zero;

            Vector3 fromTarget = fromTarget = transform.position - info.point;
            float dist = Vector3.Distance (transform.position, info.point);

            switch (forceType) {
                case ForceType.normal:
                    force = info.normal * (forwardFeelerDepth * scale / dist);
                    break;
                case ForceType.incident:
                    fromTarget.Normalize ();
                    force -= Vector3.Reflect (fromTarget, info.normal) * (forwardFeelerDepth / dist);
                    break;
                case ForceType.up:
                    force += Vector3.up * (forwardFeelerDepth * scale / dist);
                    break;
                case ForceType.braking:
                    force += fromTarget * (forwardFeelerDepth / dist);
                    break;
            }
            return force;
        }

        private void UpdateFeeler(int feelerNum, Quaternion localRotation, float baseDepth, FeelerInfo.FeeelerType feelerType) {
            Vector3 direction = localRotation * transform.rotation * Vector3.forward;
            float depth = baseDepth + ((boid.speed / boid.maxSpeed) * baseDepth * frontFeelerUpdatesPerSecond);

            RaycastHit info;
            bool collided = Physics.SphereCast (transform.position, feelerRadius, direction, out info, depth, mask.value);

            Vector3 feelerEnd = transform.position + direction * depth;

            bool collidedShip = false;

            if (collided) {
                ShipCollider shipCollider = info.collider.GetComponent<ShipCollider> ();
                if (shipCollider != null) {
                    if (shipCollider.ship != ship) {
                        feelerEnd = info.point;
                        collidedShip = true;
                    }
                }
            }

            feelers[feelerNum] = new FeelerInfo (feelerEnd, info.normal, collidedShip, feelerType);
        }

        private IEnumerator UpdateFrontFeelers() {
            yield return new WaitForSeconds (Random.Range (0.0f, 0.5f));
            while (true) {
                UpdateFeeler (0, Quaternion.identity, this.forwardFeelerDepth, FeelerInfo.FeeelerType.front);
                yield return new WaitForSeconds (1.0f / frontFeelerUpdatesPerSecond);
            }
        }

        private IEnumerator UpdateSideFeelers() {
            yield return new WaitForSeconds (Random.Range (0.0f, 0.5f));
            float angle = 45;
            while (true) {
                // Left feeler
                UpdateFeeler (1, Quaternion.AngleAxis (angle, Vector3.up), sideFeelerDepth, FeelerInfo.FeeelerType.side);
                // Right feeler
                UpdateFeeler (2, Quaternion.AngleAxis (-angle, Vector3.up), sideFeelerDepth, FeelerInfo.FeeelerType.side);
                // Up feeler
                UpdateFeeler (3, Quaternion.AngleAxis (angle, Vector3.right), sideFeelerDepth, FeelerInfo.FeeelerType.side);
                // Down feeler
                UpdateFeeler (4, Quaternion.AngleAxis (-angle, Vector3.right), sideFeelerDepth, FeelerInfo.FeeelerType.side);

                yield return new WaitForSeconds (1.0f / sideFeelerUpdatesPerSecond);
            }
        }
        #endregion

        public struct FeelerInfo {
            public Vector3 point;
            public Vector3 normal;
            public bool collided;
            public FeeelerType feelerType;

            public enum FeeelerType
            {
                front,
                side
            };

            public FeelerInfo(Vector3 point, Vector3 normal, bool collided, FeeelerType feelerType) {
                this.point = point;
                this.normal = normal;
                this.collided = collided;
                this.feelerType = feelerType;
            }
        }

        public enum ForceType {
            normal,
            incident,
            up,
            braking
        };

    }

}