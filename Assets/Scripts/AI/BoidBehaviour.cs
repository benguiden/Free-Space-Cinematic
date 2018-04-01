using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace{

    [RequireComponent (typeof (BoidActor))]
    public abstract class BoidBehaviour : MonoBehaviour {

        #region Public Variables
        [Range (0.1f, 1f)]
        public float calculationUpdateSpeed = 1f;
        public float weight = 1f;
        #endregion

        #region Protected Variables
        protected BoidActor boid;
        #endregion

        #region Private Variables
        private Coroutine calculationsCo;
        #endregion

        #region Mono Methods
        protected virtual void Awake() {
            boid = GetComponent<BoidActor> ();
            boid.behaviours.Add (this);

            if (enabled)
                calculationsCo = StartCoroutine (UpdateCalculations ());
        }

        private void OnEnable() {
            if (calculationsCo != null)
                StopCoroutine (calculationsCo);
            calculationsCo = StartCoroutine (UpdateCalculations ());
        }

        private void OnDisable() {
            if (calculationsCo != null)
                StopCoroutine (calculationsCo);
        }
        #endregion

        #region Boid Methods
        public virtual Vector3 UpdateForce() {
            return Vector3.zero;
        }

        protected virtual IEnumerator UpdateCalculations() {
            while (enabled) {
                Calculate ();
                yield return new WaitForSeconds (Time.deltaTime / calculationUpdateSpeed);
            }
        }

        protected virtual void Calculate() { }
        #endregion

    }

}
