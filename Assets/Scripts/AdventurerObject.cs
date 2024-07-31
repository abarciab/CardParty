using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Threading.Tasks;

public class AdventurerObject : Creature
{
    [Header("Sounds")]
    [SerializeField] private Sound _startDragSound;
    [SerializeField] private Sound _endDragSound;

    [Header("Adventurer Animation")]
    [SerializeField] private string _animPlaceTriggerString = "place";

    [HideInInspector] public AdventurerData AdventurerData { get; private set; }
    private bool _isBeingDragged = false;
    private bool _facingMouse = true;

    private void Start()
    {
        _startDragSound = Instantiate(_startDragSound);
        _endDragSound = Instantiate(_endDragSound);
        CardGameManager.i.OnStartPlayerTurn.AddListener(() => _facingMouse = true);
        CardGameManager.i.OnEndPlayerTurn.AddListener(OnEndTurn);
    }

    private void OnEndTurn()
    {
        _facingMouse = false;
        FaceForward(0.1f);
    }

    private async void FaceForward(float duration)
    {
        Quaternion startRot = transform.localRotation;
        float timePassed = 0;
        while (timePassed < duration) {
            timePassed += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, timePassed / duration);
            await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000));
        }
        transform.localRotation = Quaternion.identity;
    }

    private void Update() {
        if (_facingMouse) FaceMouse();

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
                if (slot.IsBlockSlot) {
                    _facingMouse = false;

                    Vector3 direction = slot.EnemyObject.transform.position - transform.position;
                    direction.y = 0; 

                    Quaternion rotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
                }
            }
        }

        if (_isBeingDragged) DoDrag(); 
    }

    private void FaceMouse()
    {
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool hit = groundPlane.Raycast(ray, out float distance);
        if (!hit) return;

        Vector3 hitPoint = ray.GetPoint(distance);
        Vector3 direction = hitPoint - transform.position;
        direction.y = 0; // Keep direction on the Y axis

        Quaternion rotation = Quaternion.LookRotation(direction);
        var targetRot = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 3 * Time.deltaTime);
    }

    public override string GetName() {
        return AdventurerData.Name;
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
        UI.Initialize(this);
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
        _startDragSound.Play();
        _facingMouse = true;
    }

    private void EndDrag() {
        Animator.SetTrigger(_animPlaceTriggerString);
        _isBeingDragged = false;
        transform.localPosition = Vector3.zero;
        _endDragSound.Play();
    }
}