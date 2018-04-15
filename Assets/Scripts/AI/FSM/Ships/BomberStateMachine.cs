using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    namespace BansheeStates {

        public class BomberAttackState : ShipState {

            public Ship emporer;

            public BomberAttackState(StateMachine _stateMachine, Ship _ship, Ship _emporer) : base(_stateMachine, _ship) {
                emporer = _emporer;
            }

            public override void Enter() {
                ship.StartCoroutine(IUpdate());
            }

            public override void Update() { }

            public override IEnumerator IUpdate() {
                yield return null;
                Pursue pursue = ship.boid.GetBehaviour<Pursue>();

                if (pursue != null)
                    pursue.target = ShipManager.main.emporer.boid;

                if (ship != null) {
                    while ((ship.enabled) && (stateMachine.state == this)) {
                        
                    }
                }
            }

            public override void Exit() {

            }

        }

    }

}