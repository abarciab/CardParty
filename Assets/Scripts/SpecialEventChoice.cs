using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SpecialEventChoice : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private TextMeshProUGUI _goodNewsText;
    [SerializeField] private Image _backing;
    public List<SpecialEventOutcome> Outcomes;
    public SpecialEventChoiceData Data;
    public bool IsEnabled = true;

    public void Initialize(SpecialEventChoiceData data)
    {
        gameObject.SetActive(true);
        _mainText.text = data.Text;
        _goodNewsText.text = data.GetPercent();
        Outcomes = data.SuccessOutcomeData.Outcomes.Concat(data.FailureOutcomeData.Outcomes).ToList();
        Data = data;
    }

    public void DisableChoice() {
        Color newColor = _backing.color;
        newColor.a = 0.25f;
        _backing.color = newColor;
        IsEnabled = false;
    }

    public void EnableChoice() {
        Color newColor = _backing.color;
        newColor.a = 1f;
        _backing.color = newColor;
        IsEnabled = true;
    }
}
