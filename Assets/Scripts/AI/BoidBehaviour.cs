using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace{

    [System.Serializable]
    public abstract class BoidBehaviour{

        #region Public Variables
        public bool enabled = true;
        #endregion

        #region Protected Variables
        protected BoidActor boid;
        #endregion

        #region Constructors
        public BoidBehaviour(BoidActor boidActor) {
            boid = boidActor;
        }
        #endregion

        #region Data Methods
        public void SetBoidActor(BoidActor boidActor) {
            boid = boidActor;
        }
        #endregion

        #region Boid Methods
        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void OnDrawGizmos() { }
        public virtual void OnValidate() { }
        #endregion

    }

}
