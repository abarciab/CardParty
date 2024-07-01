using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCameraPlane : MonoBehaviour
{
    [SerializeField] private Camera _cam;

    void Awake() {
        _cam = Camera.main;
    }
    void Update() {
        transform.rotation = _cam.transform.rotation;
    }
}
