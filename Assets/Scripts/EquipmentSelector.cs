using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSelector : MonoBehaviour
{
    [SerializeField] private List<EquipmentSelectable> _selectableCoordinators = new List<EquipmentSelectable>();
    [SerializeField] private OverworldPartyController _controller;

    public void OpenOrnamentSelector() => OpenSelector(EquipmentSlot.ORNAMENT);
    public void OpenArmorSelector() => OpenSelector(EquipmentSlot.ARMOR);
    public void OpenMainSelector() => OpenSelector(EquipmentSlot.MAIN);

    private void OpenSelector (EquipmentSlot slot)
    {
        var equipmentList = new List<Equipment> (PlayerInfo.Inventory.GetValidItems(slot));
        var currentlyEquipped = PlayerInfo.Party.GetCurrentEquipment(_controller.CurrentAdventurer, slot);
        if (currentlyEquipped != null) {
            print("currently equipped: " +  currentlyEquipped);
            equipmentList.Remove(currentlyEquipped);
        }

        for (int i = 0; i < _selectableCoordinators.Count; i++) {
            if (i >= equipmentList.Count) {
                _selectableCoordinators[i].gameObject.SetActive(false);
                continue;
            }

            var equipment = equipmentList[i];
            var owner = PlayerInfo.Party.GetOwner(equipment);
            _selectableCoordinators[i].Initialize(equipment, owner);
        }
        gameObject.SetActive(true);
    }

    public void SelectEquipment(Equipment data, AdventurerData oldUser = null)
    {
        _controller.SetEquipmentForCurrentAdventurer(data);
        gameObject.SetActive(false);
    }
}