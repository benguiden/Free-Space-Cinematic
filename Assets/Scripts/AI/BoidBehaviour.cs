using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace{

    [RequireComponent (typeof (BoidActor))]
    public abstract class BoidBehaviour : MonoBehaviour{

        #region Protected Variables
        protected BoidActor boid;
        #endregion

        #region Mono Methods
        protected virtual void Awake() {
            boid = GetComponent<BoidActor> ();
            boid.behaviours.Add (this);
        }
        #endregion

        #region Boid Methods
        public virtual void UpdateCalculations() { }
        #endregion

    }

}
