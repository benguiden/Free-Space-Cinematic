using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    namespace BansheeStates {

        public class BomberAttackState : ShipState {

            public Ship emporer;
            private Pursue pursueBehaviour;
            public float desiredAccuracy = 10f; //The threshold in degrees for the facing angle between the target ship to be under before shooting

            public BomberAttackState(StateMachine _stateMachine, Ship _ship, Ship _emporer) : base(_stateMachine, _ship) {
                emporer = _emporer;
            }

            public override void Enter() {
                ship.StartCoroutine(IUpdate());

                pursueBehaviour = ship.boid.GetBehaviour<Pursue> ();
            }

            public override void Update() { }

            public override IEnumerator IUpdate() {
                yield return null;
                Pursue pursue = ship.boid.GetBehaviour<Pursue>();

                if (pursue != null)
                    pursue.target = ShipManager.main.emporer.boid;

                if (ship != null) {
                    while ((ship.enabled) && (stateMachine.state == this)) {
                        if (pursueBehaviour != null) {
                            if (Vector3.Distance (ship.transform.position, emporer.transform.position) <= pursueBehaviour.desiredDistance) {
                                if (Vector3.Angle (ship.transform.forward, emporer.transform.position - ship.transform.position) <= desiredAccuracy) {
                                    for (int i=0; i<ship.guns.Length; i++) {
                                        ship.guns[i].AttemptShoot ();
                                    }
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