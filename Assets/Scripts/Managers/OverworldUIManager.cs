using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OverworldUIManager : UIManager
{
    public new static OverworldUIManager i;
    protected override void Awake() { base.Awake(); i = this; }

    [SerializeField] private Animator _wipe;
    [SerializeField] private SpecialEventController _eventController;
    [SerializeField] private InventoryUI _inventory;
    [SerializeField] private ShopController _shop;
    [SerializeField] private MapController _map;

    [Header("TEMP")]
    [SerializeField, DisplayInspector] private List<SpecialEventData> _specialEvents = new List<SpecialEventData>();

    public void RevealMapSprite(Vector2Int ID, Sprite sprite, int turns) => _map.RevealTile(ID, sprite, turns);
    public void EnterTileOnMap(Vector2Int ID) => _map.UpdatePlayerPosition(ID);
    public void Createmap() => _map.Initialize();

    public void OpenMap()
    {
        OpenMenus += 1;
        _map.OpenMap();
    }

    public void CloseMap()
    {
        OpenMenus -= 1;
        _map.CloseMap();
    }

    public async Task WipeScreen(float duration)
    {
        _wipe.gameObject.SetActive(true);
        await Task.Delay((int)(duration * 1000)); 
        _wipe.SetTrigger("exit");
    }

    public void StartRandomEvent()
    {
        OpenMenus += 1;
        _eventController.ShowEvent(_specialEvents[Random.Range(0, _specialEvents.Count)]);
    }

    public void OpenShop()
    {
        _shop.OpenShop();
        OpenMenus += 1;
    }

    public void CloseShop()
    {
        OpenMenus -= 1;
        _shop.gameObject.SetActive(false);
    }

    public void CloseSpecialEvent()
    {
        OpenMenus -= 1;
        _eventController.Close();
    }

    public void OpenInventory()
    {
        _inventory.Show();
        OpenMenus += 1;
    }

    public void CloseInventory()
    {
        OpenMenus -= 1;
        _inventory.gameObject.SetActive(false);
    }
 }
