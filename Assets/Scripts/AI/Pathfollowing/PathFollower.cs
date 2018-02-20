using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace{

    [System.Serializable]
    public class PathFollower : BoidBehaviour{

        //Change To Job System At Some Point

        #region Public Variables
        [Header ("Path")]
        public PathDatabase pathDatabase;
        public uint pathIndex = 0;
        public Vector3 pathPositionOffset;

        [Header ("Movement")]
        public uint pointIndex;
        public float followSpeed = 5f;
        public float pointChangeDistance = 5f;
        #endregion

        #region Private Variables
        private Path path = null;
        public Path Path { get { return path; } }
        #endregion

        #region Constructors
        public PathFollower(BoidActor boidActor) : base (boidActor) {

        }
        #endregion

        #region Boid Methods
        public override void Awake() {
            RefreshPath ();
        }

        public override void Update() {
            if (path != null) {
                CheckNextPoint ();
                LookAtPoint ();
                MoveTowardsPoint ();
            }
        }

        public override void OnDrawGizmos() {
            if (path != null) {
                if (pointIndex < path.points.Count) {
                    Gizmos.color = new Color (path.color.r, path.color.g, path.color.b, path.color.a / 2f);
                    int lastPoint = (int)pointIndex - 1;
                    if (lastPoint < 0)
                        lastPoint = path.points.Count - 1;
                    Gizmos.DrawWireCube (path.points[lastPoint] + pathPositionOffset, new Vector3 (2f, 2f, 2f));
                    Gizmos.DrawLine (path.points[lastPoint] + pathPositionOffset, path.points[(int)pointIndex] + pathPositionOffset);
                    Gizmos.color = path.color;
                    Gizmos.DrawWireCube (path.points[(int)pointIndex] + pathPositionOffset, new Vector3 (2f, 2f, 2f));

                    Vector3 lookTarget = path.points[(int)pointIndex] + pathPositionOffset;
                    lookTarget -= boid.transform.position;
                    Gizmos.DrawLine (boid.transform.position, boid.transform.position + (lookTarget.normalized * 10f));
                }
            }
        }
        #endregion

        #region Movement Methods
        private void RefreshPath() {
            if (pathDatabase == null) {
                Debug.LogWarning ("Warning: Path Database reference on " + boid.gameObject.name + " set to null.\n");
                return;
            } else {
                if (pathIndex >= pathDatabase.paths.Count) {
                    Debug.LogError ("Error: Path index out of range for " + boid.gameObject.name + " game object.\n");
                    Debug.Break ();
                    return;
                } else {
                    path = pathDatabase.paths[(int)pathIndex];
                }
            }
        }

        private void CheckNextPoint() {
            if (Vector3.Distance(boid.transform.position, path.points[(int)pointIndex] + pathPositionOffset) <= pointChangeDistance) {
                pointIndex = (uint)((pointIndex + 1) % path.points.Count);
            }
        }

        private void LookAtPoint() {
            Vector3 lookTarget = path.points[(int)pointIndex] + pathPositionOffset;
            lookTarget -= boid.transform.position;
            lookTarget.Normalize ();
            boid.transform.rotation = Quaternion.Lerp (boid.transform.rotation, Quaternion.LookRotation (lookTarget), 0.25f * Time.deltaTime * 15f);
        }

        private void MoveTowardsPoint() {
            boid.AddForwardAcceleration (followSpeed);
        }
        #endregion

        
    }
    

}