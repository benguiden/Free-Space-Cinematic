using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    public class IntercepterShip : Ship {

        #region Public Variables
        [Header ("Intercepter")]
        public Ship leader;
        #endregion

        #region Mono Methods
        protected override void Awake() {
            base.Awake ();
        }

        private void Start() {
            shipID = ShipManager.main.AddShip (this);
            stateMachine = new ShipStateMachine (this);

            if ((leader == this) || (leader == null)) {
                stateMachine.ChangeState (new IntercepterStates.IntercepterEmporerState (stateMachine, this, ShipManager.main.emporer));
            } else {
                stateMachine.ChangeState (new IntercepterStates.IntercepterEscortState (stateMachine, this, leader.boid));
                boid.GetBehaviour<OffsetPursue> ().RefreshOffset ();
            }
        }

        private void OnEnable() {
            if ((leader == this) || (leader == null)) {
                stateMachine.ChangeState (new IntercepterStates.IntercepterEmporerState (stateMachine, this, ShipManager.main.emporer));
            } else {
                stateMachine.ChangeState (new IntercepterStates.IntercepterEscortState (stateMachine, this, leader.boid));
                boid.GetBehaviour<OffsetPursue> ().RefreshOffset ();
            }
        }
        #endregion

    }

}