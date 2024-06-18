using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnClickOnCollider : MonoBehaviour
{
    [SerializeField] private UnityEvent OnClickOn = new UnityEvent();

    void Update()
    {
        CheckIfClickedOn();    
    }

    private void CheckIfClickedOn()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitPoint = Physics.Raycast(mouseRay, out var hitData);
        if (!hitPoint) return;

        var OnClick = hitData.collider.GetComponentInParent<OnClickOnCollider>();
        if (OnClick == this) OnClickOn.Invoke();
    }
}
