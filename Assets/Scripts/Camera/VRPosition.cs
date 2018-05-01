using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPosition : MonoBehaviour {

    private void OnEnabled() {
        if (VRController.main == null)
            VRController.main = FindObjectOfType<VRController> ();

        VRController.main.vrPositions.Add (transform);
    }

    private void Start() {
        if (VRController.main == null)
            VRController.main = FindObjectOfType<VRController> ();

        if (VRController.main != null)
            VRController.main.vrPositions.Add (transform);
    }

}
