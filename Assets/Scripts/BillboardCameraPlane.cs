using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCameraPlane : MonoBehaviour
{
    void Update() {
        transform.rotation = CardGameManager.i.cam.transform.rotation;
    }
}
