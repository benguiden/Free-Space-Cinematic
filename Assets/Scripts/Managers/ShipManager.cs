using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    public class ShipManager : MonoBehaviour {

        #region Public Variables
        public static ShipManager main;

        public Dictionary<uint, Ship> ships = new Dictionary<uint, Ship>();
        public Ship emporer;
        public CameraAngle emporerCameraAngle;
        public GameObject vasudansShips;
        #endregion

        #region Private Variables
        private uint shipIDIndex = 0;
        private Coroutine cinematicShipSo;
        private int sequenceState = 0;//Show emporer and Terrans, 1 - Just show Vasudans, 2 - Show Both
        #endregion

        #region Mono Methods
        private void Awake() {
            main = this;
        }

        private void Start() {
            vasudansShips.SetActive (false);

            if (cinematicShipSo != null)
                StopCoroutine (cinematicShipSo);

            cinematicShipSo = StartCoroutine (ICinematicShips (14f));
            StartCoroutine (ISceneSequence ());
        }

        private void OnEnable() {
            if (cinematicShipSo != null)
                StopCoroutine (cinematicShipSo);

            cinematicShipSo = StartCoroutine (ICinematicShips (0.1f));
        }
        #endregion

        #region Public Methods
        public uint AddShip(Ship ship) {
            ships.Add(shipIDIndex, ship);
            shipIDIndex++;
            return shipIDIndex - 1;
        }

        public void RemoveShip(uint shipID) {
            ships.Remove(shipID);
        }

        public Ship BiggestThreat(Vector3 threatPosition, Ship.Faction friendlyFaction, float startingDistance) {
            float closestDistance = startingDistance;
            Ship threatShip = null;

            foreach (KeyValuePair<uint, Ship> otherShip in ShipManager.main.ships) {
                if ((otherShip.Value.faction != friendlyFaction) && (otherShip.Value != emporer) && (otherShip.Value.isActiveAndEnabled)) {
                    float otherShipDistance = Vector3.Distance (otherShip.Value.transform.position, threatPosition); //The biggest this variable the less dangerous the ship is
                    otherShipDistance += otherShip.Value.pursuers * 250f; //The more pursuers the ship has the less the ship's dangerous
                    if (otherShipDistance < closestDistance) {
                        threatShip = otherShip.Value;
                        closestDistance = otherShipDistance;
                    }
                }
            }

            return threatShip;
        }
        #endregion

        private IEnumerator ICinematicShips(float delay) {
            yield return null;
            yield return new WaitForSeconds (delay);
            CameraAngle cameraAngle = new CameraAngle ();
            cameraAngle.interest = 1f;
            
            while ((enabled) && (ships.Count > 0)) {
                if ((Random.Range(0f, 1f) >= 0.25f) || (sequenceState != 2)) {
                    Ship randomShip = null;
                    switch (sequenceState) {
                        case 0:
                            randomShip = GetRandomShip (Ship.Faction.Terrans);
                            break;
                        case 1:
                            randomShip = GetRandomShip (Ship.Faction.Vasudans);
                            break;
                        case 2:
                            randomShip = GetRandomShip ();
                            break;
                    }

                    if (randomShip != null) {
                        cameraAngle.timeRange = new Vector2 (3f, 6f);
                        cameraAngle.interestTime = 5f;
                        if (Random.Range (0f, 1f) < 0.5f)
                            cameraAngle.stationary = true;
                        else
                            cameraAngle.stationary = false;
                        if (Random.Range (0f, 1f) < 0.5f)
                            cameraAngle.localOffset = true;
                        else
                            cameraAngle.localOffset = false;
                        cameraAngle.focus = randomShip.transform;

                        Director.main.AddAngle(cameraAngle);
                    }
                } else {
                    Director.main.AddAngle(emporerCameraAngle);
                }

                yield return new WaitForSeconds (Random.Range (3f, 5f));
            }

            cinematicShipSo = null;
        }

        private Ship GetRandomShip() {
            if (ships.Count > 0) {
                int randomIndex = Random.Range (0, ships.Count);
                foreach (Ship currentShip in ships.Values) {
                    if (randomIndex > 0)
                        randomIndex--;
                    else
                        return currentShip;
                }
            }

            return null;
        }

        private Ship GetRandomShip(Ship.Faction shipFaction) {
            if (ships.Count > 0) {
                int randomIndex = 0;
                foreach (Ship currentShip in ships.Values) {
                    if (currentShip.faction == shipFaction)
                        randomIndex++;
                }
                if (randomIndex > 0) {
                    randomIndex = Random.Range (0, randomIndex);
                    foreach (Ship currentShip in ships.Values) {
                        if (currentShip.faction == shipFaction) {
                            if (randomIndex > 0)
                                randomIndex--;
                            else
                                return currentShip;
                        }
                    }
                }
            }

            return null;
        }

        private IEnumerator ISceneSequence() {
            sequenceState = 0;
            yield return new WaitForSeconds (60f);
            Scene2 ();
            yield return new WaitForSeconds (16f);
            Scene3 ();
        }

        private void Scene2() {
            Debug.Log ("Scene 2");
            sequenceState = 1;
            vasudansShips.SetActive (true);

            Ship randomShip = GetRandomShip (Ship.Faction.Vasudans);
            if (randomShip != null) {
                CameraAngle newCameraAngle = new CameraAngle ();
                newCameraAngle.fovRange = new Vector2 (65f, 80f);
                newCameraAngle.timeRange = new Vector2 (6f, 8f);
                newCameraAngle.interestTime = 5f;
                newCameraAngle.stationary = true;
                newCameraAngle.localOffset = true;
                newCameraAngle.focus = randomShip.transform;

                Director.main.cameraAngles = new List<CameraAngle> ();
                Director.main.NewCameraAngle (newCameraAngle);
            }
        }

        private void Scene3() {
            Debug.Log ("Scene 3");
            sequenceState = 2;

            Ship randomShip = GetRandomShip ();
            if (randomShip != null) {
                CameraAngle newCameraAngle = new CameraAngle ();
                newCameraAngle.distanceRange = new Vector2 (3500f, 4500f);
                newCameraAngle.fovRange = new Vector2 (45f, 60f);
                newCameraAngle.timeRange = new Vector2 (6f, 8f);
                newCameraAngle.interestTime = 5f;
                newCameraAngle.stationary = true;
                newCameraAngle.localOffset = true;
                newCameraAngle.focus = randomShip.transform;
                Director.main.NewCameraAngle (newCameraAngle);
            }
        }

    }

}