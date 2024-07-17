using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [Header("TopRight")]
    [SerializeField] private GameObject _gridItemPrefab;
    [SerializeField] private Transform _gridParent;
    [Space(10)]
    [SerializeField] private GameObject _adventurerCardParent;
    [SerializeField] private List<PlayableCardDisplay> _adventurerCards = new List<PlayableCardDisplay>();
    [Space(10)]
    [SerializeField] private GameObject _adventurerHealthParent;
    [SerializeField] private List<GameObject> _adventurerHealthCoords = new List<GameObject>();

    [Space(10), Header("HoveredCard")]
    [SerializeField] private PlayableCardDisplay _hoveredCard;
    [Space(10)]
    [SerializeField] private GameObject _adventurerProfileCardParent;
    [SerializeField] private Image _adventurerPortrait;
    [SerializeField] private TextMeshProUGUI _adventurerName;
    [SerializeField] private TextMeshProUGUI _adventurerHealth;
    [SerializeField] private TextMeshProUGUI _adventurerDescription;

    [Header("AdventurerSection")]
    [SerializeField] private GameObject _adventurerSectionParent;
    [SerializeField] private GameObject _orText;
    [SerializeField] private GameObject _hireNewButton;

    private List<ShopGridItem> _gridItems = new List<ShopGridItem>();
    private CardData _currentHoveredCard;
    private AdventurerData _potentialNewHire;
    private ShopData _currentShopData;

    private void Start()
    {
        OverworldManager.i.OnNewTileEntered.AddListener(() => _adventurerSectionParent.SetActive(true));
    }

    public void ShowNewHire()
    {
        _gridParent.gameObject.SetActive(false);
        _hoveredCard.gameObject.SetActive(false);
        _adventurerCardParent.SetActive(true);

        _adventurerProfileCardParent.SetActive(true);
        _adventurerPortrait.sprite = _potentialNewHire.portrait;
        _adventurerName.text = _potentialNewHire.Name;
        _adventurerHealth.text = "Health: " + _potentialNewHire.MaxHealth;
        _adventurerDescription.text = _potentialNewHire.Description;
        var cards = _potentialNewHire.GetInnateCards(3);
        for (int i = 0; i < 3; i++) {
            _adventurerCards[i].Initialize(cards[i]);
        }
    }

    public void HideNewHire()
    {
        _gridParent.gameObject.SetActive(true);
        _adventurerProfileCardParent.SetActive(false);
        _adventurerCardParent.SetActive(false);
    }

    public void HireNewAdventurer()
    {
        PlayerInfo.Party.AddAdventurer(_potentialNewHire);
        _adventurerSectionParent.SetActive(false);
        HideNewHire();
        _currentShopData.AdventuerOptionUsed = true;
    }

    public void ShowHealthStats()
    {
        _gridParent.gameObject.SetActive(false);
        _hoveredCard.gameObject.SetActive(false);
        _adventurerHealthParent.SetActive(true);

        var adventurers = PlayerInfo.Party.Adventurers;
        for (int i = 0; i < _adventurerHealthCoords.Count; i++) {
            var coord = _adventurerHealthCoords[i];
            if (i >= adventurers.Count) {
                coord.SetActive(false);
                continue;
            }

            var adventurer = adventurers[i];
            coord.GetComponentInChildren<Slider>().value = adventurer.Stats.HealthPercent;
            coord.GetComponentInChildren<TextMeshProUGUI>().text = adventurer.Name;
            coord.SetActive(true);    
        }
    }

    public void HideHealthStats()
    {
        _gridParent.gameObject.SetActive(true);
        _adventurerHealthParent.SetActive(false);
    }

    public void HealAllAdventurers()
    {
        PlayerInfo.Party.HealAllAdventurers();
        _adventurerSectionParent.SetActive(false);
        HideHealthStats();
        _currentShopData.AdventuerOptionUsed = true;
    }

    public void OpenShop(ShopData data)
    {
        ClearGridItems();
        if (data.ItemOptions.Count > data.NumItems) data.initializeItemList();

        _currentShopData = data;
        var options = data.ItemOptions;
        foreach (var item in options) ShowItem(item);

        if (data.AdventuerOptionUsed) _adventurerSectionParent.SetActive(false);
        else {
            _adventurerSectionParent.SetActive(true);
            ConfigureAdventurerTab(data.AdventurerOptions);
        }

        gameObject.SetActive(true);
    }

    private void ConfigureAdventurerTab(List<AdventurerData> adventurers)
    {
        _adventurerSectionParent.SetActive(true);
        var validOptions = adventurers.Where(x => !PlayerInfo.Party.Adventurers.Contains(x)).ToList();
        if (validOptions.Count == 0) {
            SetStateNewAdventurerButton(false);
            return;
        }

        _potentialNewHire = validOptions[Random.Range(0, validOptions.Count)];
        SetStateNewAdventurerButton(true);
    }

    private void SetStateNewAdventurerButton(bool shown)
    {
        _orText.SetActive(shown);
        _hireNewButton.SetActive(shown);
    }

    public void ShowCard(CardData card)
    {
        _currentHoveredCard = card;
        _hoveredCard.Initialize(card);
    }

    public void StopShowingCard(CardData card)
    {
        if (_currentHoveredCard != card) return;
        _hoveredCard.gameObject.SetActive(false);
        _currentHoveredCard = null;
    }

    private void ClearGridItems()
    {
        foreach (var item in _gridItems) if (item) Destroy(item.gameObject);
        _gridItems.Clear();
    }

    private void ShowItem(Equipment item)
    {
        var newGridItem = Instantiate(_gridItemPrefab, _gridParent);
        var itemScript = newGridItem.GetComponent<ShopGridItem>();
        itemScript.Initialize(item, this);
        _gridItems.Add(itemScript);
    }

    public void BuyItem(ShopGridItem gridItem)
    {
        var toBuy = gridItem.Equipment;
        Destroy(gridItem.gameObject);

        PlayerInfo.Stats.Money -= toBuy.Cost;
        PlayerInfo.Inventory.AddEquipment(toBuy);
        _currentShopData.ItemOptions.Remove(toBuy);

        RecalculateCosts();
    }

    private void RecalculateCosts()
    {
        foreach (var item in _gridItems) item.RefreshButton();
    }

    public void CloseShop()
    {
        OverworldUIManager.i.CloseShop();
    }
}
