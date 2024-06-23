using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class Adventurer : Creature
{
    public AdventurerData adventurerData;
    public override void OnPointerUp(PointerEventData eventData) {
        if (eventData.pointerId == -1) {
            //deselect if currently selected
            if (CardGameManager.i.selectedCreatures.Contains(this)) {
                Deselect();
            } else {
                Select();
            }
        }

        base.OnPointerUp(eventData);
    }
}