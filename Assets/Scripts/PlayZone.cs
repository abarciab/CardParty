using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData) {
        CardGameManager.i.HoveredPlayZone = this;
    }
 
    public void OnPointerExit(PointerEventData eventData) {
        if (CardGameManager.i.HoveredPlayZone == this) CardGameManager.i.HoveredPlayZone = null;
    }
}
