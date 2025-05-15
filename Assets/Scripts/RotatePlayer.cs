using System;
using UnityEngine;

public class RotatePlayer : MonoBehaviour {
    private Camera cam;
    private Transform head, body;

    private void Start() {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        head = transform.Find("Head").transform;
        body = transform.Find("Body").transform;
    }

    private void Update() {
        if (cam == null) return;
        if (GameQuery.OnGetIsCheckingCameras?.Invoke() ?? false) return;
        
 
            Vector3 camEuler = cam.transform.localEulerAngles;

            // Normalize angles to -180 to 180
            camEuler.x = NormalizeAngle(camEuler.x);
            camEuler.y = NormalizeAngle(camEuler.y);
            camEuler.z = NormalizeAngle(camEuler.z);

            // Clamp if necessary (optional here)
            camEuler.x = Mathf.Clamp(camEuler.x, -180f, 180f);
            camEuler.y = Mathf.Clamp(camEuler.y, -180f, 180f);
            camEuler.z = Mathf.Clamp(camEuler.z, -180f, 180f);

            // Apply body rotation (X and Z only)
            body.localEulerAngles = new Vector3(0f, camEuler.y, 0f);

            // Apply head rotation (full)
            head.localEulerAngles = new Vector3(camEuler.x, camEuler.y, camEuler.z);
     
    }

    private float NormalizeAngle(float angle) {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}