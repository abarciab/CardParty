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
    [SerializeField] private List<PartyEquipmentUI> _equipmentCoordinators = new List<PartyEquipmentUI>();

    [Header("Innate Tab")]
    [SerializeField] List<PlayableCardDisplay> _innateCards = new List<PlayableCardDisplay>();

    [Header("Details Tab")]
    [SerializeField] private TextMeshProUGUI _detailsName;
    [SerializeField] private Image _detailsPortrait;
    [SerializeField] private TextMeshProUGUI _detailsDescription;

    public AdventurerData CurrentAdventurer { get; private set; }

    public void SetEquipmentForCurrentAdventurer(Equipment data)
    {
        PlayerInfo.Party.SetEquipment(CurrentAdventurer, data, data.Slot);
        UpdateDisplay();
    }

    public void OpenParty()
    {
        var adventurers = PlayerInfo.Party.Adventurers;
        for (int i = 0; i < _adventurerCoordinators.Count; i++) DisplayAdventurerInList(i < adventurers.Count ? adventurers[i] : null, i);
        CurrentAdventurer = adventurers[0];
        _adventurerCoordinators[0].GetComponent<SelectableItem>().Select();

        UpdateDisplay();

        gameObject.SetActive(true);
    }

    private void UpdateDisplay()
    {
        UpdateEquipmentTab();
        UpdateInnateTab();
        UpdateDetailsTab();
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
