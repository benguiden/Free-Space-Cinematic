using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    namespace BansheeStates {

        public class BansheeWanderState : ShipState {

            public Ship emporer;
            public float threatDistance = 3000f;

            public BansheeWanderState(StateMachine _stateMachine, Ship _ship, Ship _emporer) : base(_stateMachine, _ship) {
                emporer = _emporer;
            }

            public override void Enter() {
                if (ship != null)
                    ship.StartCoroutine(IUpdate());
            }

            public override void Update() { }

            public override IEnumerator IUpdate() {
                if (ship != null) {
                    while ((ship.enabled) && (stateMachine.state == this)) {
                        Vector3 emporerPosition = ShipManager.main.emporer.transform.position;

                        float closestDistance = threatDistance;
                        Ship threatShip = null;

                        foreach (KeyValuePair<uint, Ship> otherShip in ShipManager.main.ships) {
                            float otherShipDistance = Vector3.Distance(otherShip.Value.transform.position, emporerPosition);
                            Debug.Log(otherShipDistance);
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

            public override void Exit() { }

        }

        public class BansheePersueState : ShipState {

            public Ship target;
            public float targetDesiredDistance = 150f;
            private Pursue pursueBehaviour;

            public BansheePersueState(StateMachine _stateMachine, Ship _ship, Ship threat) : base(_stateMachine, _ship) {
                target = threat;
            }

            public override void Enter() {
                pursueBehaviour = target.boid.GetBehaviour<Pursue>();
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