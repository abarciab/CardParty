using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class AdventurerObject : Creature
{
    public AdventurerData AdventurerData { get; private set; }
    private bool _isBeingDragged = false;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (CardGameManager.i.CurrCombatState == CombatState.PlayerTurn && !CardGameManager.i.CurrentPlayedCard && IsHover()) {
                StartDrag();
            }
        }

        if (Input.GetMouseButtonUp(0) && _isBeingDragged) {
            CombatSlot slot = GetHoveredCombatSlot();
            
            EndDrag();
            
            if (slot) {
                slot.SetCreature(this);
            }
        }

        if (_isBeingDragged) DoDrag(); 
    }

    private void DoDrag()
    {
        var pos = GetMouseRayPoint();
        transform.position = new Vector3(pos.x, 2, pos.z);
    }

    private Vector3 GetMouseRayPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 currentPos = transform.position;
        Vector3 camForward = -Camera.main.transform.forward;
        float rayDist = Vector3.Dot(transform.position - ray.origin, camForward) / Vector3.Dot(ray.direction, camForward);
        Vector3 pos = ray.origin + ray.direction * rayDist;
        return pos;
    }

    public void Initialize(AdventurerData data)
    {
        AdventurerData = data;
        gameObject.name = data.name;
    }

    private bool IsHover() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, 100);
        foreach (RaycastHit hit in hits) {
            if (hit.transform.gameObject.GetComponent<AdventurerObject>() == this) return true;
        }

        return false;
    }

    private CombatSlot GetHoveredCombatSlot() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, 100);
        foreach (RaycastHit hit in hits) {
            if (hit.transform.gameObject.GetComponent<CombatSlot>()) return hit.transform.gameObject.GetComponent<CombatSlot>();
        }

        return null;
    }

    private void StartDrag() {
        _isBeingDragged = true;
    }

    private void EndDrag() {
        _isBeingDragged = false;
        transform.localPosition = Vector3.zero;
    }
}