using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CameraAudioOptimisation : MonoBehaviour {

    private AudioSource audioSource;

    private Coroutine updateCo;

    private void Awake() {
        audioSource = GetComponent<AudioSource> ();
    }

    private void Start() {
        if (updateCo != null)
            StopCoroutine (updateCo);

        updateCo = StartCoroutine (IUpdate (0.1f));
    }

    private void OnEnable() {
        if (updateCo != null)
            StopCoroutine (updateCo);

        updateCo = StartCoroutine (IUpdate (0.1f));
    }

    private IEnumerator IUpdate(float updateRate) {
        yield return new WaitForSeconds (Random.Range (0f, updateRate));
        while ((enabled) && (Camera.main != null)) {
            float cameraDistance = Vector3.Distance (transform.position, Camera.main.transform.position);
            if ((cameraDistance > audioSource.maxDistance) && (audioSource.isPlaying)) {
                audioSource.Pause ();
            } else if ((cameraDistance <= audioSource.maxDistance) && (!audioSource.isPlaying)) {
                audioSource.priority = (int)cameraDistance;
                audioSource.UnPause ();
            }
            yield return new WaitForSeconds (updateRate);
        }
    }

}
