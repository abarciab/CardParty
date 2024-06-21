using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moneyText;

    private void Update()
    {
        _moneyText.text = "Money: " + PlayerInfo.Stats.Money;
    }
}
