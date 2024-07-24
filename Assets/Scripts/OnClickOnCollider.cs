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

    public void OverrideOnClickOn(UnityAction callback)
    {
        OnClickOn = new UnityEvent();
        OnClickOn.AddListener(callback);
    }

    void Update()
    {
        if (!UIManager.i.IsBusy) CheckIfClickedOn();    
    }

    private void CheckIfClickedOn()
    {
        var currentHovered = GameManager.i.CurrentHoveredOnCollider;

        if (currentHovered == this) {
            if (!_isMouseOver) OnPointerEnter.Invoke();
            if (Input.GetMouseButtonDown(0)) OnClickOn.Invoke();
        }
        else { 
            if (_isMouseOver) OnPointerExit.Invoke();
        }
    }
}
