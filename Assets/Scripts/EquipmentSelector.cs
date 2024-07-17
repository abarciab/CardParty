using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class EquipmentSelector : MonoBehaviour
{
    [SerializeField] private List<EquipmentSelectable> _selectableCoordinators = new List<EquipmentSelectable>();
    [SerializeField] private OverworldPartyController _controller;
    [SerializeField] private GameObject _nextPage;
    [SerializeField] private GameObject _prevPage;
    private int _currentPage;
    private int _maxPages;
    private const int _equipmentPerPage = 5;
    List<Equipment> _currentList = new List<Equipment>();
    private bool _selecting;

    public void OpenOrnamentSelector() => OpenSelector(EquipmentSlot.ORNAMENT);
    public void OpenArmorSelector() => OpenSelector(EquipmentSlot.ARMOR);
    public void OpenMainSelector() => OpenSelector(EquipmentSlot.MAIN);

    private void OpenSelector (EquipmentSlot slot)
    {
        _selecting = true;
        _currentList = new List<Equipment> (PlayerInfo.Inventory.GetValidItems(slot));
        if (_currentList.Count <= 0) return;

        _maxPages = Mathf.CeilToInt(_currentList.Count / (float)_equipmentPerPage);
        _currentPage = 0;

        UpdateDisplay();

        gameObject.SetActive(true);
    }

    public void NextPage()
    {
        _currentPage += 1;
        UpdateDisplay();
    }

    public void PreviousPage()
    {
        _currentPage -= 1;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        _nextPage.SetActive(_currentPage <  _maxPages - 1);
        _prevPage.SetActive(_currentPage > 0);

        DisplayCurrentPage();
    }

    private void DisplayCurrentPage()
    {
        for (int i = 0; i < _selectableCoordinators.Count; i++) {
            int equipmentIndex = i + (_currentPage * _equipmentPerPage);
            if (equipmentIndex >= _currentList.Count) {
                _selectableCoordinators[i].gameObject.SetActive(false);
                continue;
            }

            var equipment = _currentList[equipmentIndex];
            var owner = PlayerInfo.Party.GetOwner(equipment);
            _selectableCoordinators[i].Initialize(equipment, owner);
        }
    }

    public void SelectEquipment(Equipment data, AdventurerData oldUser = null)
    {
        _controller.SetEquipmentForCurrentAdventurer(data);
        _selecting = false;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (_selecting) CancelSelection();
    }

    public void CancelSelection()
    {
        _controller.SetEquipmentForCurrentAdventurer();
        _selecting = false;
        gameObject.SetActive(false);
    }
}