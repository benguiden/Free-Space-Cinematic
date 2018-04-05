using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {

    public ComponentType componentType;

    public bool onlyDeactivate = false;

    private Component component;

    private bool destroyed = false;

    private void Awake() {
        switch (componentType) {
            case ComponentType.ParticleSystem:
                component = GetComponent (typeof (ParticleSystem));
                break;
        }
        if (component == null) {
            Debug.LogError ("Error: Auto Destroy missing specified component type.");
        }
    }

    private void Update() {
        if (!destroyed) {
            switch (componentType) {
                case ComponentType.ParticleSystem:
                    if (((ParticleSystem)component).isStopped)
                        DestroyGameObject ();
                    break;
            }
        }
    }

    private void DestroyGameObject() {
        destroyed = true;
        if (onlyDeactivate)
            gameObject.SetActive (false);
        else
            Destroy (gameObject);
    }

    public enum ComponentType{
        ParticleSystem
    }

}
