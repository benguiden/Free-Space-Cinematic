using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{
    [RequireComponent(typeof(LineRenderer))]
    public class WarpGateVFX : MonoBehaviour {

        #region Public Variables
        [Header ("Visuals")]
        public float radius = 250f;
        public float radiusBulge = 25f;
        public int resolution = 32;
        public float frequency = 1f;
        public float form = 16f;
        #endregion

        #region Private Variables
        private LineRenderer lineRenderer;
        private float theta = 0f;
        #endregion

        #region Mono Methods
        private void Awake() {
            lineRenderer = GetComponent<LineRenderer> ();
        }

        private void OnDrawGizmosSelected() {
            if (!Application.isPlaying) {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere (transform.position, radius);
            }
        }

        private void OnValidate() {
            if (lineRenderer != null)
                lineRenderer.positionCount = resolution;
        }

        private void Start() {
            lineRenderer.positionCount = resolution;
        }

        private void Update() {
            CalculateLinePositions ();
            theta += frequency * Mathf.PI * 2f * Time.deltaTime;
        }
        #endregion

        #region VFX Methods
        private void CalculateLinePositions() {
            if (lineRenderer.positionCount > 0) {
                for (int i = 0; i < lineRenderer.positionCount; i++) {
                    lineRenderer.SetPosition (i, CalculateLinePosition (i / (float)lineRenderer.positionCount));
                }
            }
        }

        private Vector3 CalculateLinePosition(float fraction) {
            Vector3 pointPosition = Vector3.zero;
            float angle = fraction * 2f * Mathf.PI;

            float amp = radius;
            amp += Mathf.Sin (theta + (fraction * Mathf.PI * form)) * radiusBulge;
            amp -= Mathf.Cos ((theta * 2f) + (fraction * Mathf.PI * form)) * radiusBulge * 0.25f;

            pointPosition.x = amp * Mathf.Cos (angle);
            pointPosition.y = amp * Mathf.Sin (angle);
            pointPosition.z = amp * (radiusBulge / 500f);

            return pointPosition;
        }
        #endregion

    }

}