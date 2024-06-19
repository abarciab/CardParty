using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        gameObject.SetActive(false);
    }
}
