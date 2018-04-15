using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    public class BomberShip : Ship {

        #region Mono Methods
        private void Start() {
            shipID = ShipManager.main.AddShip(this);
            stateMachine = new ShipStateMachine(this);
            stateMachine.ChangeState(new BansheeStates.BomberAttackState(stateMachine, this, ShipManager.main.emporer));
        }
        #endregion

    }

}