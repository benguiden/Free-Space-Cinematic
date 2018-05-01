using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreeSpace
{

    public class Director : MonoBehaviour {

        #region Public Variables
        public static Director main;

        public int maxAngles = 30;

        [Header("References")]
        public Camera mainCamera;
        public LookAt lookAtComponent;

        //[HideInInspector]
        public List<CameraAngle> cameraAngles = new List<CameraAngle> ();
        public CameraAngle currentAngle = null;
        #endregion

        #region Private Variables
        //Current Camera Angle
        private float angleTimeLeft, angleFov;
        private Vector3 angleOffset;
        private Vector3 lastTargetPosition = new Vector3 ();
        #endregion

        #region Mono Methods
        private void Awake() {
            main = this;

            if (mainCamera == null) {
                Debug.LogError ("Error: Camera missing from Director component");
                Debug.Break ();
            }
        }

        private void Start() {
            NewCameraAngle (currentAngle);
        }

        private void Update() {
            ClampAngles ();
            UpdateAngleTimes ();

            UpdateCurrentAngle ();

            if (Input.GetKeyDown (KeyCode.Space))
                NewCameraAngle (currentAngle);
        }
        #endregion

        #region List Methods
        private CameraAngle GetBestAngle() {
            CameraAngle cameraAngle = null;

            float bestInterest = 0f;

            for (int i = cameraAngles.Count - 1; i >= 0; i--) {
                if (cameraAngles[i].focus != null) {
                    if (cameraAngles[i].interest > bestInterest) {
                        bestInterest = cameraAngles[i].interest;
                        cameraAngle = cameraAngles[i];
                    }
                } else {
                    cameraAngles.RemoveAt (i);
                }
            }

            return cameraAngle;
        }

        private void ClampAngles() {
            if (cameraAngles.Count > 30) {
                cameraAngles.RemoveRange (0, cameraAngles.Count - 30);
            }
        }

        private void UpdateAngleTimes() {
            for (int i = cameraAngles.Count - 1; i >= 0; i--) {
                if (cameraAngles[i].focus != null) {
                    cameraAngles[i].interestTime -= Time.deltaTime;
                    if (cameraAngles[i].interestTime <= 0f)
                        cameraAngles.RemoveAt (i);
                } else {
                    cameraAngles.RemoveAt (i);
                }
            }
        }
        #endregion

        #region Angle Methods
        public void AddAngle(CameraAngle newAngle) {
            cameraAngles.Add (newAngle);
        }

        public void NewCameraAngle(CameraAngle newAngle) {
            angleTimeLeft = Random.Range (newAngle.timeRange.x, newAngle.timeRange.y);
            angleFov = Random.Range (newAngle.fovRange.x, newAngle.fovRange.y);

            angleOffset = new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), Random.Range (-1f, 1f));
            angleOffset *= Random.Range (newAngle.distanceRange.x, newAngle.distanceRange.y);

            if (currentAngle.localOffset)
                mainCamera.transform.position = newAngle.focus.position + angleOffset;
            else
                mainCamera.transform.position = newAngle.focus.TransformPoint (angleOffset);

            mainCamera.fieldOfView = angleFov;

            lookAtComponent.NewTarget (newAngle.focus);

            currentAngle = newAngle;
        }

        private void UpdateCurrentAngle() {
            if (currentAngle.focus != null) {
                lastTargetPosition = currentAngle.focus.position;
                angleTimeLeft -= Time.deltaTime;
                if (angleTimeLeft > 0f) {
                    if (!currentAngle.stationary) {
                        if (currentAngle.localOffset)
                            mainCamera.transform.position = currentAngle.focus.position + angleOffset;
                        else
                            mainCamera.transform.position = currentAngle.focus.TransformPoint (angleOffset);
                    }
                } else {
                    ChangeAngles ();
                }
            } else {
                ZoomOut ();
            }
        }

        private void ZoomOut() {
            currentAngle.focus = (new GameObject ("Ship Explosion Position")).transform;
            currentAngle.focus.position = lastTargetPosition;
            angleTimeLeft = Random.Range (currentAngle.timeRange.x, currentAngle.timeRange.y);
            angleFov = currentAngle.fovRange.x;

            angleOffset = currentAngle.distanceRange.y * new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), Random.Range (-1f, 1f));

            if (currentAngle.localOffset)
                mainCamera.transform.position = currentAngle.focus.position + angleOffset;
            else
                mainCamera.transform.position = currentAngle.focus.TransformPoint (angleOffset);

            mainCamera.fieldOfView = angleFov;

            lookAtComponent.NewTarget(currentAngle.focus);
        }

        private void ChangeAngles() {
            if (cameraAngles.Count == 0) {
                NewCameraAngle (currentAngle);
            } else {
                NewCameraAngle (GetBestAngle ());
            }
        }
        #endregion

    }

    [System.Serializable]
    public class CameraAngle {
        public Transform focus = null;
        public bool localOffset = false;
        public bool stationary = false;
        public Vector2 timeRange = new Vector2 (2f, 5f);
        public Vector2 distanceRange = new Vector2 (25f, 100f);
        public Vector2 fovRange = new Vector2 (30f, 75f);
        public float interest = 10f;
        public float interestTime = 5f;

        public CameraAngle() { }
    }

}