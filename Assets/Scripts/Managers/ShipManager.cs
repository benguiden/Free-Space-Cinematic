using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    public class ShipManager : MonoBehaviour {

        #region Public Variables
        public static ShipManager main;

        public Dictionary<uint, Ship> ships = new Dictionary<uint, Ship>();
        public Ship emporer;
        #endregion

        #region Private Variables
        private uint shipIDIndex = 0;
        #endregion

        #region Mono Methods
        private void Awake() {
            main = this;
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
        #endregion

    }

}