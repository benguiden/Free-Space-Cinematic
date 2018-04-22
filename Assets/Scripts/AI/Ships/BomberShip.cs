using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    public class BomberShip : Ship {

        [Header("Bomber")]
        public MissileLauncher missileLauncher;

        #region Mono Methods
        protected override void Awake() {
            base.Awake();
            missileLauncher.ship = this;
        }

        private void Start() {
            shipID = ShipManager.main.AddShip(this);
            stateMachine = new ShipStateMachine(this);
            stateMachine.ChangeState(new BansheeStates.BomberAttackState(stateMachine, this, ShipManager.main.emporer));
        }
        #endregion

    }

}