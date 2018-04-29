using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    [RequireComponent(typeof(Renderer))]
    public class Debris : MonoBehaviour{

        public Vector3 flyDirection = Vector3.up;

        private Coroutine activated;

        public void Activate(Vector3 explositionPosition, float velocity, float lifeTime) {
            if (activated != null)
                StopCoroutine (activated);

            activated = StartCoroutine (IActivate (explositionPosition, velocity, lifeTime));
        }

        private IEnumerator IActivate(Vector3 explositionPosition, float velocity, float lifeTime) {
            Vector3 newVelocity = transform.TransformDirection (flyDirection).normalized * velocity;
            while ((lifeTime > 0f) && (isActiveAndEnabled)) {
                transform.position += newVelocity * Time.deltaTime;

                lifeTime -= Time.deltaTime;
                yield return null;
            }
            Destroy (gameObject);
        }

    }

}
