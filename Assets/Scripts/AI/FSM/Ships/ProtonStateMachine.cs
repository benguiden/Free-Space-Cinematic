using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    namespace ProtonStates {

        public class ProtonPatrolState : ShipState {

            public float threatDistance = 2500f;
            public OffsetPursue patrolBehaviour;

            public ProtonPatrolState(StateMachine _stateMachine, Ship _ship, BoidActor _leader) : base(_stateMachine, _ship) {
                patrolBehaviour = ship.boid.GetBehaviour<OffsetPursue> ();

                if (patrolBehaviour != null)
                    patrolBehaviour.leader = _leader;
            }

            public override void Enter() {
                ship.StartCoroutine(IUpdate());

                patrolBehaviour = ship.boid.GetBehaviour<OffsetPursue> ();
                Pursue pursueBehaviour = ship.boid.GetBehaviour<Pursue> ();

                Wander wanderBehaviour = ship.boid.GetBehaviour<Wander> ();

                if (wanderBehaviour != null) {
                    wanderBehaviour.enabled = true;
                    wanderBehaviour.weight = 0.25f;
                }

                if (patrolBehaviour != null)
                    patrolBehaviour.enabled = true;

                if (pursueBehaviour != null)
                    pursueBehaviour.enabled = false;
            }

            public override void Update() { }

            public override IEnumerator IUpdate() {
                yield return null;
                if (ship != null) {
                    while ((ship.enabled) && (stateMachine.state == this)) {
                        if (ShipManager.main.emporer == null) {

                        } else {
                            Vector3 emporerPosition = ShipManager.main.emporer.transform.position;

                            Ship threatShip = ShipManager.main.BiggestThreat (ship.transform.position, ship.faction, threatDistance);
                            if (threatShip != null) {
                                stateMachine.ChangeState (new ProtonPersueState (stateMachine, ship, threatShip));
                            } else {
                                yield return new WaitForSeconds (updateRefresh);
                            }
                        }
                    }
                }
            }

            public override void Exit() {
                if (patrolBehaviour != null)
                    patrolBehaviour.enabled = false;
            }

            public override string ToString() {
                return "Patrol";
            }

        }

        public class ProtonPersueState : ShipState {

            private Ship target;
            private float targetDesiredDistance = 250f;
            private Pursue pursueBehaviour;
            private float maxDistance = 2500f;
            private float desiredAccuracy = 25f; //The threshold in degrees for the facing angle between the target ship to be under before shooting

            public ProtonPersueState(StateMachine _stateMachine, Ship _ship, Ship threat) : base(_stateMachine, _ship) {
                target = threat;
            }

            public override void Enter() {
                Wander wanderBehaviour = ship.boid.GetBehaviour<Wander> ();
                if (wanderBehaviour != null) {
                    wanderBehaviour.enabled = true;
                    wanderBehaviour.weight = 10f;
                }

                pursueBehaviour = ship.boid.GetBehaviour<Pursue> ();
                if (pursueBehaviour != null) {
                    pursueBehaviour.enabled = true;
                    pursueBehaviour.target = target.boid;
                    pursueBehaviour.desiredDistance = targetDesiredDistance;
                }

                Flee targetFlee = target.boid.GetBehaviour<Flee>();

                if (targetFlee != null)
                    targetFlee.avoidingBoids.Add(ship.transform);

                target.pursuers++;
                ship.pursuing = target;

                ship.guns[0].enabled = true;

                ship.StartCoroutine (IUpdate ());
            }

            public override void Update() {
                
            }

            public override IEnumerator IUpdate() {
                yield return null;
                if (ship != null) {
                    while ((ship.enabled) && (stateMachine.state == this)) {
                        if (target != null) {
                            if (Vector3.Distance (ship.transform.position, target.transform.position) >= maxDistance) {
                                stateMachine.ChangeState (new ProtonPatrolState (stateMachine, ship, ShipManager.main.emporer.boid));
                            } else if (ship.guns[0].AimingAt (target.boid, desiredAccuracy)) {
                                ship.guns[0].AttemptShoot ();
                            }
                            yield return null;
                        } else {
                            stateMachine.ChangeState (new ProtonPatrolState (stateMachine, ship, ShipManager.main.emporer.boid));
                        }
                    }
                }
            }

            public override void Exit() {
                Flee targetFlee = target.boid.GetBehaviour<Flee>();

                if (targetFlee != null)
                    targetFlee.avoidingBoids.Remove(ship.transform);

                if (ship.pursuing != null)
                    ship.pursuing.pursuers--;
            }

            public override string ToString() {
                if (target != null)
                    return "Persue " + target.name;
                else
                    return "Persue";
            }

        }

    }

}