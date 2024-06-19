using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnClickOnCollider : MonoBehaviour
{
    [SerializeField] private UnityEvent OnClickOn = new UnityEvent();
    [SerializeField] private UnityEvent OnPointerEnter = new UnityEvent();
    [SerializeField] private UnityEvent OnPointerExit = new UnityEvent();

    private bool _isMouseOver;

    private void Start()
    {
        OnPointerEnter.AddListener(() => { _isMouseOver = true; });
        OnPointerExit.AddListener(() => { _isMouseOver = false; });
    }

    void Update()
    {
        if (!UIManager.i.IsBusy) CheckIfClickedOn();    
    }

    private void CheckIfClickedOn()
    {
        bool justClicked = Input.GetMouseButtonDown(0);

        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitPoint = Physics.Raycast(mouseRay, out var hitData);
        if (!hitPoint) {
            if (_isMouseOver) OnPointerExit.Invoke();
            return;
        }

        var OnClick = hitData.collider.GetComponentInParent<OnClickOnCollider>();
        if (OnClick == this) {
            if (justClicked) OnClickOn.Invoke();
            if (!_isMouseOver) OnPointerEnter.Invoke();
        }
        else if (_isMouseOver) OnPointerExit.Invoke();
    }
}
