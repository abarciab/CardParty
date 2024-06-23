using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Creature creature;
    public void OnPointerEnter(PointerEventData eventData) {
        CardGameManager.i.hoveredPlayZone = this;
    }
 
    public void OnPointerExit(PointerEventData eventData) {
        if (CardGameManager.i.hoveredPlayZone == this) CardGameManager.i.hoveredPlayZone = null;
    }
}
