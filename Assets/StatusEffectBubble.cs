using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatusEffectBubble : MonoBehaviour
{
    public StatusEffectType Type;
    public Image Image;
    public TMP_Text Text;
    public void Initialize(StatusEffectType newType) {
        Type = newType;
        Image.sprite = Resources.Load<Sprite>("Images/UI/StatusEffects/" + Type.ToString());
        Text.text = Type.ToString();
    }
}
