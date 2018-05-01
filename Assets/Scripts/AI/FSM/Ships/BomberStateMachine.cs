using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    namespace BomberStates {

        public class BomberAttackState : ShipState {

            private Ship emporer;
            private Arrive arriveBehaviour;
            public float desiredAccuracy = 25f; //The threshold in degrees for the facing angle between the target ship to be under before shooting
            private bool missileCam = true;

            public BomberAttackState(StateMachine _stateMachine, Ship _ship, Ship _emporer) : base(_stateMachine, _ship) {
                emporer = _emporer;
            }

            public override void Enter() {
                ship.StartCoroutine(IUpdate());

                arriveBehaviour = ship.boid.GetBehaviour<Arrive> ();
                if (arriveBehaviour != null)
                    arriveBehaviour.enabled = true;

                Flee fleeBehaviour = ship.boid.GetBehaviour<Flee> ();
                if (fleeBehaviour != null)
                    fleeBehaviour.enabled = false;
            }

            public override void Update() { }

            public override IEnumerator IUpdate() {
                yield return null;
                if (arriveBehaviour != null)
                    arriveBehaviour.target = ShipManager.main.emporer.transform;

                if (ship != null) {
                    while ((ship.enabled) && (stateMachine.state == this)) {
                        if (arriveBehaviour != null)
                            arriveBehaviour.target = ShipManager.main.emporer.transform;
                        if ((ShipManager.main.emporer == null) || (((BomberShip)ship).missileLauncher.MissileIndex <= ((BomberShip)ship).missileLauncher.missiles.Length - 1)) {
                            if (arriveBehaviour != null) {
                                if (Vector3.Distance (ship.transform.position, emporer.transform.position) <= arriveBehaviour.nearingDistance * 1.1f) {
                                    if (Vector3.Angle (ship.transform.forward, emporer.transform.position - ship.transform.position) <= desiredAccuracy) {
                                        ((BomberShip)ship).missileLauncher.AttemptShoot ();
                                        ShowMissileCam ();
                                    }
                                }
                            }
                        } else {
                            stateMachine.ChangeState (new BomberFleeState (stateMachine, ship, emporer));
                        }
                        yield return null;
                    }
                }
            }

            public override void Exit() {
                if (arriveBehaviour != null)
                    arriveBehaviour.enabled = false;
            }

            public override string ToString() {
                return "Attack Emporer";
            }

            private void ShowMissileCam() {
                if (missileCam) {
                    missileCam = false;
                    CameraAngle newCameraAngle = new CameraAngle ();
                    newCameraAngle.fovRange = new Vector2 (75f, 85f);
                    newCameraAngle.distanceRange = new Vector2 (45f, 60f);
                    newCameraAngle.timeRange = new Vector2 (8f, 10f);
                    newCameraAngle.interestTime = 15f;
                    newCameraAngle.interest = 25f;
                    newCameraAngle.stationary = false;
                    newCameraAngle.localOffset = false;
                    MissileLauncher missileLauncher = ((BomberShip)ship).missileLauncher;
                    newCameraAngle.focus = missileLauncher.missiles[missileLauncher.MissileIndex].transform;
                    Director.main.AddAngle (newCameraAngle);
                }
            }

        }

        public class BomberFleeState : ShipState
        {

            private Ship emporer;
            private Flee fleeBehaviour;

            public BomberFleeState(StateMachine _stateMachine, Ship _ship, Ship _emporer) : base (_stateMachine, _ship) {
                emporer = _emporer;
            }

            public override void Enter() {
                ship.StartCoroutine (IUpdate ());

                fleeBehaviour = ship.boid.GetBehaviour<Flee> ();

                if (fleeBehaviour != null) {
                    fleeBehaviour.avoidingBoids.Add (emporer.transform);
                    fleeBehaviour.enabled = true;
                }
            }

            public override void Update() { }

            public override IEnumerator IUpdate() {
                yield return null;
            }

            public override void Exit() { }

            public override string ToString() {
                return "Fleeing";
            }

        }

    }

}