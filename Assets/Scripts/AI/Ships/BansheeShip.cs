using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    public class BansheeShip : Ship {

        #region Mono Methods
        private void Start() {
            shipID = ShipManager.main.AddShip(this);
            stateMachine = new ShipStateMachine(this);
            stateMachine.ChangeState(new BansheeStates.BansheeWanderState(stateMachine, this, ShipManager.main.emporer));
        }
        #endregion

    }

}