using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRController : MonoBehaviour {

    public static VRController main;

    public bool onOverView = true;

   // [HideInInspector]
    public List<Transform> vrPositions;

    

    private void Awake() {
        main = this;
    }

    void Start () {
        if (!onOverView)
            StartCoroutine (IChangeCameras ());
	}

    private IEnumerator IChangeCameras() {
        while (isActiveAndEnabled) {
            Transform newTransform = vrPositions[Random.Range (0, vrPositions.Count)];
            transform.parent = newTransform.parent;
            transform.localPosition = newTransform.localPosition;
            transform.localRotation = newTransform.localRotation;
            transform.localScale = newTransform.localScale;
            yield return new WaitForSeconds (15f);
        }
    }

}
