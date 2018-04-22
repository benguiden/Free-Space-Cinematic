using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    public class BansheeShip : Ship {

        #region Public Variables
        [Header ("Banshee")]
        public Ship leader;
        #endregion

        #region Mono Methods
        private void Start() {
            shipID = ShipManager.main.AddShip(this);
            stateMachine = new ShipStateMachine(this);

            if ((leader == null) && (leader != this))
                stateMachine.ChangeState (new BansheeStates.BansheePatrolState (stateMachine, this, ShipManager.main.emporer));
            else {
                stateMachine.ChangeState (new BansheeStates.BansheeFollowLeader (stateMachine, this, leader));
                boid.GetBehaviour<OffsetPursue> ().RefreshOffset ();
            }
        }
        #endregion

    }

}