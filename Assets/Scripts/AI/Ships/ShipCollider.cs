using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    public class ShipCollider : MonoBehaviour {

        [HideInInspector]
        public Ship ship;

        public void Initalise(Ship parentShip) {
            ship = parentShip;
            gameObject.tag = "ShipCollider";
        }

        private void Start() {
            
        }

    }

}
