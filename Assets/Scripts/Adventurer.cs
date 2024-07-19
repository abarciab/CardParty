using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class Adventurer : Creature
{
    public AdventurerData AdventurerData { get; private set; }
    private bool _isBeingDragged = false;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (CardGameManager.i.CurrCombatState == CombatState.PlayerTurn && !CardGameManager.i.CurrPlayedCard && IsHover()) {
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

        if (_isBeingDragged) {
            //https://gist.github.com/SimonDarksideJ/477f5674285b63cba8e752c43950ed7c
            Ray R = Camera.main.ScreenPointToRay(Input.mousePosition); // Get the ray from mouse position
            Vector3 PO = transform.position; // Take current position of this draggable object as Plane's Origin
            Vector3 PN = -Camera.main.transform.forward; // Take current negative camera's forward as Plane's Normal
            float t = Vector3.Dot(transform.position - R.origin, PN) / Vector3.Dot(R.direction, PN); // plane vs. line intersection in algebric form. It find t as distance from the camera of the new point in the ray's direction.
            Vector3 P = R.origin + R.direction * t; // Find the new point.

            transform.position = new Vector3(P.x, 5, P.z);
        }
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
            if (hit.transform.gameObject.GetComponent<Adventurer>() == this) return true;
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