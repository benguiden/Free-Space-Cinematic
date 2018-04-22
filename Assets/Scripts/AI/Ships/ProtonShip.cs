using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    public class ProtonShip : Ship
    {

        #region Public Variables
        //[Header ("Banshee")]
        //public Ship leader;
        #endregion

        #region Mono Methods
        private void Start() {
            shipID = ShipManager.main.AddShip (this);
            stateMachine = new ShipStateMachine (this);

            stateMachine.ChangeState (new ProtonStates.ProtonPatrolState (stateMachine, this, ShipManager.main.emporer.boid));
        }
        #endregion

    }

}