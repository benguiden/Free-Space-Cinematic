using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    namespace IntercepterStates {

        public class IntercepterEscortState : ShipState {

            private OffsetPursue offsetPursueBehaviour;
            private BoidActor escortingShip;
            private float threatDistance = 1250f;

            public IntercepterEscortState(StateMachine _stateMachine, Ship _ship, BoidActor _escortingShip) : base (_stateMachine, _ship) {
                escortingShip = _escortingShip;
            }

            public override void Enter() {
                updateRefresh = 0.5f;

                offsetPursueBehaviour = ship.boid.GetBehaviour<OffsetPursue> ();

                if ((offsetPursueBehaviour == null) || (escortingShip == null))
                    AttackEmporer ();
                else {
                    offsetPursueBehaviour.enabled = true;
                    offsetPursueBehaviour.leader = escortingShip;
                    offsetPursueBehaviour.RefreshOffset ();
                }

                ship.StartCoroutine (IUpdate ());
            }

            public override void Update() {
                if (escortingShip == null)
                    AttackEmporer ();
            }

            public override IEnumerator IUpdate() {
                yield return null;
                if (ship != null) {
                    while ((ship.enabled) && (stateMachine.state == this)) {
                        if (escortingShip != null) {
                            Ship threat = ShipManager.main.BiggestThreat (escortingShip.transform.position, ship.faction, threatDistance);
                            if ((threat != null) && (threat != ShipManager.main.emporer)) {
                                stateMachine.ChangeState (new IntercepterPursueState (stateMachine, ship, threat));
                            } else if (Vector3.Distance(escortingShip.transform.position, ShipManager.main.emporer.transform.position) <= 2500f){
                                stateMachine.ChangeState (new IntercepterEmporerState (stateMachine, ship, ShipManager.main.emporer));
                            }
                        } else {
                            AttackEmporer ();
                        }
                        yield return new WaitForSeconds (updateRefresh);
                    }
                }
            }

            public override void Exit() {
                if (offsetPursueBehaviour != null)
                    offsetPursueBehaviour.enabled = false;
            }

            public override string ToString() {
                return "Escort";
            }

            private void AttackEmporer() {
                stateMachine.ChangeState (new IntercepterEmporerState (stateMachine, ship, ShipManager.main.emporer));
            }

        }

        public class IntercepterEmporerState : ShipState {

            private Ship emporer;
            private Pursue pursueBehaviour;
            private float emporerDistance = 1500f;
            private float threatDistance = 1250f;
            public float desiredAccuracy = 15f; //The threshold in degrees for the facing angle between the target ship to be under before shooting

            public IntercepterEmporerState(StateMachine _stateMachine, Ship _ship, Ship _emporer) : base (_stateMachine, _ship) {
                emporer = _emporer;
            }

            public override void Enter() {
                pursueBehaviour = ship.boid.GetBehaviour<Pursue> ();

                if (pursueBehaviour != null) {
                    pursueBehaviour.enabled = true;
                    pursueBehaviour.target = emporer.boid;
                    pursueBehaviour.desiredDistance = emporerDistance;
                }

                ship.guns[0].enabled = true;

                ship.StartCoroutine (IUpdate ());
            }

            public override void Update() {
                if (emporer == null) {
                    //Flee
                }
            }

            public override IEnumerator IUpdate() {
                yield return null;
                if (ship != null) {
                    while ((ship.enabled) && (stateMachine.state == this)) {
                        if (emporer != null) {
                            if (ship.guns[0].AimingAt (emporer.boid, desiredAccuracy)) {
                                ship.guns[0].AttemptShoot ();
                            }

                            Ship threat = ShipManager.main.BiggestThreat (ship.transform.position, ship.faction, threatDistance);
                            if ((threat != null) && (threat != ShipManager.main.emporer)) {
                                stateMachine.ChangeState (new IntercepterPursueState (stateMachine, ship, threat));
                            }
                        } else {
                            //Flee
                        }
                        yield return null;
                    }
                }
            }

            public override void Exit() {
                if (pursueBehaviour != null)
                    pursueBehaviour.enabled = false;
            }

            public override string ToString() {
                return "Attack Emporer";
            }

        }

        public class IntercepterPursueState : ShipState {

            private Ship target;
            private Pursue pursueBehaviour;
            private float desiredDistance = 100f;
            private float maxDistance = 30000f;
            public float desiredAccuracy = 5f; //The threshold in degrees for the facing angle between the target ship to be under before shooting

            public IntercepterPursueState(StateMachine _stateMachine, Ship _ship, Ship _target) : base (_stateMachine, _ship) {
                target = _target;
            }

            public override void Enter() {
                pursueBehaviour = ship.boid.GetBehaviour<Pursue> ();

                if (pursueBehaviour != null) {
                    pursueBehaviour.enabled = true;
                    pursueBehaviour.target = target.boid;
                    pursueBehaviour.desiredDistance = desiredDistance;
                }

                Flee targetFlee = target.boid.GetBehaviour<Flee> ();

                if (targetFlee != null)
                    targetFlee.avoidingBoids.Add (ship.transform);

                ship.guns[0].enabled = true;

                ship.StartCoroutine (IUpdate ());
            }

            public override void Update() {
                if (target == null) {
                    LookForBomber ();
                }
            }

            public override IEnumerator IUpdate() {
                yield return null;
                if (ship != null) {
                    while ((ship.enabled) && (stateMachine.state == this)) {
                        if (target != null) {
                            if (Vector3.Distance (ship.transform.position, target.transform.position) >= maxDistance) {
                                LookForBomber ();
                                yield return new WaitForSeconds (updateRefresh);
                            } else if (ship.guns[0].AimingAt (target.boid, desiredAccuracy)) {
                                ship.guns[0].AttemptShoot ();
                                yield return null;
                            } else {
                                yield return null;
                            }
                        } else {
                            LookForBomber ();
                            yield return new WaitForSeconds (updateRefresh);
                        }
                    }
                }
            }

            public override void Exit() {
                if (pursueBehaviour != null)
                    pursueBehaviour.enabled = false;
            }

            public override string ToString() {
                if (target != null)
                    return "Persue " + target.name;
                else
                    return "Persue";
            }

            private void LookForBomber() {
                OffsetPursue offsetPursue = ship.boid.GetBehaviour<OffsetPursue> ();
                if (offsetPursue != null) {
                    BoidActor escortingShip = offsetPursue.leader;
                    if (escortingShip != null)
                        stateMachine.ChangeState (new IntercepterEscortState (stateMachine, ship, escortingShip));
                    else
                        stateMachine.ChangeState (new IntercepterEmporerState (stateMachine, ship, ShipManager.main.emporer));
                }
            }

        }
    }
}