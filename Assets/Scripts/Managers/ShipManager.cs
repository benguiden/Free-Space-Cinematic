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

            cinematicShipSo = StartCoroutine (ICinematicShips (15f));
        }

        private void OnEnable() {
            if (cinematicShipSo != null)
                StopCoroutine (cinematicShipSo);

            cinematicShipSo = StartCoroutine (ICinematicShips (0f));
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

        private IEnumerator ICinematicShips(float delay) {
            yield return new WaitForSeconds (delay);
            CameraAngle cameraAngle = new CameraAngle ();
            cameraAngle.interest = 1f;
            cameraAngle.timeRange = new Vector2 (3f, 6f);
            
            while ((enabled) && (ships.Count > 0)) {
                if (Random.Range(0f, 1f) >= 0.25f) {
                    Ship randomShip = GetRandomShip();

                    if (randomShip != null) {
                        cameraAngle.interestTime = 2.5f;
                        if (Random.Range (0f, 1f) < 0.25f)
                            cameraAngle.stationary = true;
                        else
                            cameraAngle.stationary = false;
                        if (Random.Range (0f, 1f) < 0.25f)
                            cameraAngle.localOffset = true;
                        else
                            cameraAngle.localOffset = false;
                        cameraAngle.focus = GetRandomShip().transform;

                        Director.main.AddAngle(cameraAngle);
                    }
                } else {
                    Director.main.AddAngle(emporerCameraAngle);
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