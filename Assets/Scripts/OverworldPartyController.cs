using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverworldPartyController : MonoBehaviour
{
    [Header("Adventurer Selector")]
    [SerializeField] private List<PartyAdventurerUI> _adventurerCoordinators = new List<PartyAdventurerUI>();

    [Header("Equipment Tab")]
    [SerializeField] private SelectableItem _equipmentHeaderButton;
    [SerializeField] private List<PartyEquipmentUI> _equipmentCoordinators = new List<PartyEquipmentUI>();

    [Header("Innate Tab")]
    [SerializeField] private SelectableItem _innateHeaderButton;
    [SerializeField] List<PlayableCardDisplay> _innateCards = new List<PlayableCardDisplay>();

    [Header("Details Tab")]
    [SerializeField] private SelectableItem _detailsHeaderButton;
    [SerializeField] private TextMeshProUGUI _detailsName;
    [SerializeField] private Image _detailsPortrait;
    [SerializeField] private TextMeshProUGUI _detailsDescription;
    [SerializeField] private TextMeshProUGUI _detailsHealthText;
    [SerializeField] private Slider _detailsHealthSlider;
    [SerializeField] private GameObject _fireButton;

    public AdventurerData CurrentAdventurer { get; private set; }

    public void FireCurrentAdventurer()
    {
        PlayerInfo.Party.FireAdventurer(CurrentAdventurer);
        if (PlayerInfo.Party.Adventurers.Count == 0) {
            return;
        }

        var adventurers = PlayerInfo.Party.Adventurers;
        UpdateAdventuerers();
        CurrentAdventurer = adventurers[0];
        _adventurerCoordinators[0].GetComponent<SelectableItem>().Select();

        UpdateDisplay();
    }

    public void SetEquipmentForCurrentAdventurer(Equipment data = null)
    {
        if (data != null) PlayerInfo.Party.SetEquipment(CurrentAdventurer, data, data.Slot);
        UpdateDisplay();
    }

    public void OpenParty()
    {
        var adventurers = PlayerInfo.Party.Adventurers;
        CurrentAdventurer = adventurers[0];
        _adventurerCoordinators[0].GetComponent<SelectableItem>().Select();

        _detailsHeaderButton.Select();

        UpdateDisplay();

        gameObject.SetActive(true);
    }

    private void UpdateDisplay()
    {
        UpdateAdventuerers();
        UpdateEquipmentTab();
        UpdateInnateTab();
        UpdateDetailsTab();
    }

    private void UpdateAdventuerers()
    {
        var adventurers = PlayerInfo.Party.Adventurers;
        for (int i = 0; i < _adventurerCoordinators.Count; i++) DisplayAdventurerInList(i < adventurers.Count ? adventurers[i] : null, i);
    }

    private void UpdateEquipmentTab()
    {
        var equipment = PlayerInfo.Party.GetEquipment(CurrentAdventurer);
        for (int i = 0; i < _equipmentCoordinators.Count; i++) {
            _equipmentCoordinators[i].Initialize(equipment[i], this);
        }
    }

    private void UpdateInnateTab()
    {
        for (int i = 0; i < _innateCards.Count; i++) {
            if (i < CurrentAdventurer.Cards.Count) _innateCards[i].Initialize(CurrentAdventurer.Cards[i], CurrentAdventurer.Name);
            else _innateCards[i].gameObject.SetActive(false);
        }
    }

    private void UpdateDetailsTab()
    {
        _detailsName.text = CurrentAdventurer.Name;
        _detailsPortrait.sprite = CurrentAdventurer.portrait;
        _detailsDescription.text = CurrentAdventurer.Description;
        _detailsHealthText.text = CurrentAdventurer.Stats.HealthString;
        _detailsHealthSlider.value = CurrentAdventurer.Stats.HealthPercent;
        _fireButton.SetActive(PlayerInfo.Party.Adventurers.Count > 1);
    }

    private void DisplayAdventurerInList(AdventurerData adventurer, int i)
    {
        var coord = _adventurerCoordinators[i];
        if (adventurer == null) coord.gameObject.SetActive(false);
        else coord.Initialize(adventurer, this);
    }

    public void CloseParty()
    {
        gameObject.SetActive(false);
    }

    public void SwitchAdventurer(AdventurerData newAdventurer)
    {
        CurrentAdventurer = newAdventurer;
        UpdateDisplay();
    }
}
