using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFacing : MonoBehaviour
{
    [Tooltip("Optional. If not set, Camera.main is used.")]
    public Camera targetCamera;

    // Update is called once per frame
    void Update()
    {
        var cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
            return;

        Vector3 cameraPosition = cam.transform.position;
        transform.LookAt(cameraPosition);
        transform.Rotate(0, 180, 0);
    }
}