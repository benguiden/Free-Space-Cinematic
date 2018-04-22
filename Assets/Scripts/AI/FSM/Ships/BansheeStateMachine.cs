using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    namespace BansheeStates {

        public class BansheePatrolState : ShipState {

            public Ship emporer;
            public float threatDistance = 4000f;
            public PathFollower pathFollower;

            public BansheePatrolState(StateMachine _stateMachine, Ship _ship, Ship _emporer) : base(_stateMachine, _ship) {
                emporer = _emporer;
            }

            public override void Enter() {
                ship.StartCoroutine(IUpdate());
                updateRefresh = 1f;
                pathFollower = ship.boid.GetBehaviour<PathFollower> ();
                Wander wanderBehaviour = ship.boid.GetBehaviour<Wander> ();
                Pursue pursueBehaviour = ship.boid.GetBehaviour<Pursue> ();

                if (pathFollower != null)
                    pathFollower.enabled = true;

                if (wanderBehaviour != null)
                    wanderBehaviour.enabled = true;

                if (pursueBehaviour != null)
                    pursueBehaviour.enabled = false;
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
                            if ((otherShip.Value.faction != ship.faction)) {
                                float otherShipDistance = Vector3.Distance (otherShip.Value.transform.position, emporerPosition);
                                if ((otherShipDistance < closestDistance) && (otherShip.Value != ship)) {
                                    threatShip = otherShip.Value;
                                    closestDistance = otherShipDistance;
                                }
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

            public override string ToString() {
                return "Patrol";
            }

        }

        public class BansheeFollowLeader : ShipState
        {
            private BoidActor leaderBoid;
            private OffsetPursue offsetBehaviour;

            public BansheeFollowLeader(StateMachine _stateMachine, Ship _ship, BoidActor _leaderBoid) : base(_stateMachine, _ship) {
                leaderBoid = _leaderBoid;
            }

            public override void Enter() {
                ship.StartCoroutine (IUpdate ());

                PathFollower pathFollower = ship.boid.GetBehaviour<PathFollower> ();
                Wander wanderBehaviour = ship.boid.GetBehaviour<Wander> ();
                Pursue pursueBehaviour = ship.boid.GetBehaviour<Pursue> ();
                offsetBehaviour = ship.boid.GetBehaviour<OffsetPursue> ();

                if (pathFollower != null)
                    pathFollower.enabled = false;

                if (wanderBehaviour != null)
                    wanderBehaviour.enabled = false;

                if (pursueBehaviour != null)
                    pursueBehaviour.enabled = false;

                if (offsetBehaviour != null) {
                    offsetBehaviour.leader = leaderBoid;
                    offsetBehaviour.enabled = true;
                }
            }

            public override void Update() { }

            public override IEnumerator IUpdate() {
                yield return null;
                if (ship != null) {
                    while ((ship.enabled) && (stateMachine.state == this)) {
                        yield return null;
                    }
                }
            }

            public override void Exit() {
                if (offsetBehaviour != null) {
                    offsetBehaviour.enabled = false;
                }
            }

            public override string ToString() {
                return "Follow Leader";
            }

        }

        public class BansheePersueState : ShipState {

            public Ship target;
            public float targetDesiredDistance = 150f;
            private Pursue pursueBehaviour;
            public float desiredAccuracy = 5f; //The threshold in degrees for the facing angle between the target ship to be under before shooting

            public BansheePersueState(StateMachine _stateMachine, Ship _ship, Ship threat) : base(_stateMachine, _ship) {
                target = threat;
            }

            public override void Enter() {
                Wander wanderBehaviour = ship.boid.GetBehaviour<Wander> ();

                if (wanderBehaviour != null)
                    wanderBehaviour.enabled = true;

                pursueBehaviour = ship.boid.GetBehaviour<Pursue>();
                pursueBehaviour.enabled = true;
                pursueBehaviour.target = target.boid;
                pursueBehaviour.desiredDistance = targetDesiredDistance;

                Flee targetFlee = target.boid.GetBehaviour<Flee>();

                if (targetFlee != null)
                    targetFlee.avoidingBoids.Add(ship.transform);

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
                            if (Vector3.Angle (ship.transform.forward, target.transform.position - ship.transform.position) <= desiredAccuracy) {
                                ship.guns[0].AttemptShoot ();
                            }
                            yield return null;
                        } else {
                            stateMachine.ChangeState (new BansheePatrolState (stateMachine, ship, ShipManager.main.emporer));
                        }
                    }
                }
            }

            public override void Exit() {
                Flee targetFlee = target.boid.GetBehaviour<Flee>();

                if (targetFlee != null)
                    targetFlee.avoidingBoids.Remove(ship.transform);
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