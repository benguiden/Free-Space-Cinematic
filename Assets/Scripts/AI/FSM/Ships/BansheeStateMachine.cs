using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    namespace BansheeStates {

        public class BansheeWanderState : ShipState {

            public Ship emporer;
            public float threatDistance = 3000f;
            public PathFollower pathFollower;

            public BansheeWanderState(StateMachine _stateMachine, Ship _ship, Ship _emporer) : base(_stateMachine, _ship) {
                emporer = _emporer;
            }

            public override void Enter() {
                ship.StartCoroutine(IUpdate());
                updateRefresh = 1f;
                pathFollower = ship.GetComponent<PathFollower> ();
                pathFollower.enabled = true;
            }

            public override void Update() { }

            public override IEnumerator IUpdate() {
                yield return null;
                if (ship != null) {
                    while ((ship.enabled) && (stateMachine.state == this)) {
                        Vector3 emporerPosition = ShipManager.main.emporer.transform.position;

                        float closestDistance = threatDistance;
                        Ship threatShip = null;

                        foreach (KeyValuePair<uint, Ship> otherShip in ShipManager.main.ships) {
                            float otherShipDistance = Vector3.Distance(otherShip.Value.transform.position, emporerPosition);
                            if ((otherShipDistance < closestDistance) && (otherShip.Value != ship)) {
                                threatShip = otherShip.Value;
                                closestDistance = otherShipDistance;
                            }
                        }

                        if (threatShip != null) {
                            stateMachine.ChangeState(new BansheePersueState(stateMachine, ship, threatShip));
                        } else {
                            yield return new WaitForSeconds(updateRefresh);
                        }
                    }
                }
            }

            public override void Exit() {
                if (pathFollower == null)
                    pathFollower = ship.boid.GetBehaviour<PathFollower> ();

                pathFollower.enabled = false;
            }

        }

        public class BansheePersueState : ShipState {

            public Ship target;
            public float targetDesiredDistance = 150f;
            private Pursue pursueBehaviour;

            public BansheePersueState(StateMachine _stateMachine, Ship _ship, Ship threat) : base(_stateMachine, _ship) {
                target = threat;
            }

            public override void Enter() {
                pursueBehaviour = ship.boid.GetBehaviour<Pursue>();
                pursueBehaviour.enabled = true;
                pursueBehaviour.target = target.boid;
                pursueBehaviour.desiredDistance = targetDesiredDistance;
                Debug.LogWarning("Changed State.");
            }

            public override void Update() {
                
            }

            public override IEnumerator IUpdate() {
                return null;
            }

            public override void Exit() { }

        }

    }

}