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
        #endregion

        #region Private Variables
        private uint shipIDIndex = 0;
        private Coroutine cinematicShipSo;
        #endregion

        #region Mono Methods
        private void Awake() {
            main = this;
        }

        private void Start() {
            if (cinematicShipSo != null)
                StopCoroutine (cinematicShipSo);

            cinematicShipSo = StartCoroutine (ICinematicShips ());
        }

        private void OnEnable() {
            if (cinematicShipSo != null)
                StopCoroutine (cinematicShipSo);

            cinematicShipSo = StartCoroutine (ICinematicShips ());
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
                if ((otherShip.Value.faction != friendlyFaction) && (otherShip.Value != emporer)) {
                    float otherShipDistance = Vector3.Distance (otherShip.Value.transform.position, threatPosition);
                    if (otherShipDistance < closestDistance) {
                        threatShip = otherShip.Value;
                        closestDistance = otherShipDistance;
                    }
                }
            }

            return threatShip;
        }
        #endregion

        private IEnumerator ICinematicShips() {
            CameraAngle cameraAngle = new CameraAngle ();
            cameraAngle.interest = 1f;
            
            while ((enabled) && (ships.Count > 0)) {
                if (Random.value >= 0.25f) {
                    Ship randomShip = GetRandomShip();

                    if (randomShip != null) {
                        cameraAngle.interestTime = 2.5f;
                        if (Random.value < 0.25f)
                            cameraAngle.stationary = true;
                        if (Random.value < 0.25f)
                            cameraAngle.localOffset = true;
                        cameraAngle.focus = GetRandomShip().transform;

                        Director.main.AddAngle(cameraAngle);
                    }
                } else {
                    Director.main.AddAngle(emporerCameraAngle);
                    Debug.Log("Emporer Angle");
                }

                yield return new WaitForSeconds (Random.Range (1f, 2.5f));
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

    }

}