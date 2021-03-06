﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace{

    [AddComponentMenu ("Boid Behaviours/Path Follower")]
    [RequireComponent(typeof(BoidActor))]
    public class PathFollower : BoidBehaviour{

        #region Public Variables
        [Header ("Path")]
        public PathDatabase pathDatabase;
        public uint pathIndex = 0;
        public Transform pathParentOffset;
        public Vector3 pathPositionOffset;

        [Header ("Movement")]
        public uint pointIndex;
        public float followSpeed = 20f;
        public float pointChangeDistance = 40f;
        #endregion

        #region Private Variables
        private Path path = null;
        public Path Path { get { return path; } }
        private Vector3 targetPosition;
        #endregion

        #region Mono Methods
        protected override void Awake() {
            base.Awake ();
            RefreshPath ();
        }

        public override Vector3 UpdateForce() {
            if (path != null) {
                targetPosition = path.points[(int)pointIndex] + pathPositionOffset;
                if (pathParentOffset != null)
                    targetPosition = pathParentOffset.TransformPoint(targetPosition);

                CheckNextPoint ();
                return boid.SeekForce (targetPosition, followSpeed);
            }
            return Vector3.zero;
        }

        private void OnDrawGizmos() {
            if ((path != null) && (enabled)) {
                if (pointIndex < path.points.Count) {
                    if (boid == null)
                        boid = GetComponent<BoidActor> ();

                    Gizmos.color = new Color (path.color.r, path.color.g, path.color.b, path.color.a / 2f);
                    int lastPoint = (int)pointIndex - 1;
                    if (lastPoint < 0)
                        lastPoint = path.points.Count - 1;
                    //Gizmos.DrawWireCube (path.points[lastPoint] + pathPositionOffset, new Vector3 (2f, 2f, 2f));
                    //Gizmos.DrawLine (path.points[lastPoint] + pathPositionOffset, path.points[(int)pointIndex] + pathPositionOffset);
                    Gizmos.color = path.color;

                    Gizmos.DrawWireSphere (targetPosition, 25f);

                    Vector3 lookTarget = targetPosition;
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
            if (Vector3.Distance(boid.transform.position, targetPosition) <= pointChangeDistance) {
                pointIndex = (uint)((pointIndex + 1) % path.points.Count);
            }
        }
        #endregion
        
    }
    

}