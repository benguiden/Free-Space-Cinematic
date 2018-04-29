using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{
    public class LookAt : MonoBehaviour{

        public Transform target;
        [Range (float.Epsilon, 1f)]
        public float smoothness;

        private Vector3 targetPosition;

        private void Start() {
            if (target != null)
                targetPosition = target.position;
        }

        private void Update() {
            if (target != null) {
                targetPosition = Vector3.Lerp (targetPosition, target.position, Time.deltaTime * 60f * (1f - smoothness));
                transform.LookAt (targetPosition);
            }
        }

        public void NewTarget(Transform newTarget) {
            target = newTarget;
            targetPosition = target.position;
            transform.LookAt (targetPosition);
        }

    }
}
