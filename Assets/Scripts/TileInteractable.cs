using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TMPro;
using UnityEditorInternal;
using UnityEngine;

[System.Serializable]
public class InteractableTypeGameObjectWrapper
{
    [HideInInspector] public string name;
    public TileInteractableType Type;
    public GameObject GameObject => GetCurrentObject();

    [SerializeField, OverrideLabel("default")] private GameObject _fromDown;
    [SerializeField] private bool _altViewVersions;
    [SerializeField, ConditionalField(nameof(_altViewVersions))] private GameObject _fromRight;
    [SerializeField, ConditionalField(nameof(_altViewVersions))] private GameObject _fromTop;
    [SerializeField, ConditionalField(nameof(_altViewVersions))] private GameObject _fromLeft;
    private int _selected = 0;

    private List<GameObject> GetAllVersions() => new List<GameObject>() { _fromDown, _fromRight, _fromTop, _fromLeft };

    public void Initialize(Quaternion rot)
    {
        if (rot == Quaternion.identity) return;
        if (Vector3.Distance(rot.eulerAngles, new Vector3(0, 90, 0)) < 0.1f) _selected = 1;
        if (Vector3.Distance(rot.eulerAngles, new Vector3(0, 180, 0)) < 0.1f) _selected = 2;
        if (Vector3.Distance(rot.eulerAngles, new Vector3(0, 270, 0)) < 0.1f) _selected = 3;
    }

    private GameObject GetCurrentObject()
    {
        var objects = GetAllVersions();
        var selected = objects[_selected];
        return selected ? selected : _fromDown;
    }

    public void HideAll()
    {
        foreach (var version in GetAllVersions()) if (version) version.SetActive(false);
    }
}

public class TileInteractable : MonoBehaviour
{
    [SerializeField] private List<InteractableTypeGameObjectWrapper> _objects = new List<InteractableTypeGameObjectWrapper>();
    [ReadOnly] public TileInteractableData Data;
    [SerializeField] private Transform _label;

    private void OnValidate()
    {
        foreach (var o in _objects) o.name = o.Type.ToString();
    }

    private InteractableTypeGameObjectWrapper GetCurrentObject()
    {
        return _objects.Where(x => x.Type == Data.Type).First();
    }

    public Vector3 GetCurrentObjPos()
    {
        var current = GetCurrentObject();
        return current.GameObject.transform.position;
    }

    public void Initialize(TileInteractableData data, TileController controller, Quaternion rot)
    {
        Data = data;

        foreach (var o in _objects) {
            if (o.Type == Data.Type) {
                o.Initialize(rot);
                var pos = _label.position;
                pos.x = o.GameObject.transform.position.x;
                pos.z = o.GameObject.transform.position.z;
                _label.position = pos;
                _label.GetComponentInChildren<TextMeshProUGUI>().text = data.Name;
                _label.gameObject.SetActive(data.Name.Length > 0);
            }
            o.HideAll();
            o.GameObject.SetActive(o.Type == data.Type);
        }

        GetComponent<OnClickOnCollider>().OverrideOnClickOn(() => controller.ClickOnInteractable(data));
    }

    public void PointerEnter() => SetHighlightVisiblity(true);

    public void PointerExit() => SetHighlightVisiblity(false);

    private void SetHighlightVisiblity(bool isHighlighted)
    {
        var current = GetCurrentObject();
        current.GameObject.transform.GetChild(0).gameObject.SetActive(!isHighlighted);
        current.GameObject.transform.GetChild(1).gameObject.SetActive(isHighlighted);
    }
}
