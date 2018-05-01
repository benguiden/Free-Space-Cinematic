using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CameraAudioOptimisation : MonoBehaviour {

    private AudioSource audioSource;

    private void Update() {
        audioSource.priority = (int)Vector3.Distance (transform.position, Camera.main.transform.position);
    }

}
