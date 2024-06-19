using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpecialEventChoice : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private TextMeshProUGUI _goodNewsText;

    public void Initialize(SpecialEventChoiceData data)
    {
        gameObject.SetActive(true);
        _mainText.text = data.Text;
        _goodNewsText.text = data.GetPercent();
    }
}
