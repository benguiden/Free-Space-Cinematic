using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    namespace BansheeStates {

        public class BomberAttackState : ShipState {

            public Ship emporer;
            private Arrive arriveBehaviour;
            public float desiredAccuracy = 25f; //The threshold in degrees for the facing angle between the target ship to be under before shooting

            public BomberAttackState(StateMachine _stateMachine, Ship _ship, Ship _emporer) : base(_stateMachine, _ship) {
                emporer = _emporer;
            }

            public override void Enter() {
                ship.StartCoroutine(IUpdate());

                arriveBehaviour = ship.boid.GetBehaviour<Arrive> ();
            }

            public override void Update() { }

            public override IEnumerator IUpdate() {
                yield return null;
                if (arriveBehaviour != null)
                    arriveBehaviour.target = ShipManager.main.emporer.transform;

                if (ship != null) {
                    while ((ship.enabled) && (stateMachine.state == this)) {
                        if (arriveBehaviour != null) {
                            if (Vector3.Distance (ship.transform.position, emporer.transform.position) <= arriveBehaviour.nearingDistance) {
                                if (Vector3.Angle (ship.transform.forward, emporer.transform.position - ship.transform.position) <= desiredAccuracy) {
                                    ((BomberShip)ship).missileLauncher.AttemptShoot();
                                }
                            }
                        }
                        yield return null;
                    }
                }
            }

            public override void Exit() {

            }

        }

    }

}