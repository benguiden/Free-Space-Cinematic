using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace {

    [System.Serializable]
    public class MissileLauncher : Gun {

        private int missileIndex = 0;
        public Projectile[] missiles;

        public override void Shoot() {
            if (enabled) {
                if (missileIndex <= missiles.Length - 1) {
                    Projectile missile = missiles[missileIndex];
                    missile.enabled = true;
                    Debug.Log("Enabled");
                    missile.transform.SetParent(null);
                    missile.sourceShip = ship;
                    missileIndex++;
                }
            }
        }

    }

}