using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAtStart : MonoBehaviour
{
    private void OnEnable()
    {
        if (!GameManager.i) gameObject.SetActive(false);
    }
}
