using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialEventController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _promptText;
    [SerializeField] private Image _image;
    [SerializeField] private List<SpecialEventChoice> _choices = new List<SpecialEventChoice>();
    [SerializeField] private GameObject _continueButton;

    private List<SpecialEventChoiceData> _currentChoices = new List<SpecialEventChoiceData>();
    private List<SpecialEventOutcome> _decidedOutcomes = new List<SpecialEventOutcome>();

    public void ShowEvent(SpecialEventData data)
    {
        gameObject.SetActive(true);
        _continueButton.SetActive(false);

        _titleText.text = data.Title;
        _promptText.text = data.Prompt;
        _image.sprite = data.Sprite;

        _currentChoices = ShuffleList(data.Choices);

        for (int i = 0; i < _choices.Count; i++) {
            if (data.ChoicesToDisplay > i) _choices[i].Initialize(_currentChoices[i]);
            else _choices[i].gameObject.SetActive(false);
        }
    }

    private List<SpecialEventChoiceData> ShuffleList(List<SpecialEventChoiceData> inputList)
    {
        var old = new List<SpecialEventChoiceData>(inputList);
        var shuffled = new List<SpecialEventChoiceData>();
        while (old.Count > 0) {
            var index = Random.Range(0, old.Count);
            shuffled.Add(old[index]);
            old.RemoveAt(index);
        }
        return shuffled;
    }

    public void SelectChoice(int index)
    {
        var selectedChoice = _currentChoices[index];
        bool succeeded = Random.Range(0, 1f) < selectedChoice.SuccessChance;

        _decidedOutcomes = succeeded ? selectedChoice.SucessOutcomes : selectedChoice.FailureOutcomes;
        _titleText.text = succeeded ? "Success" : "Failure";
        _promptText.text = succeeded ? selectedChoice.SuccessText : selectedChoice.FailText;
        foreach (var b in _choices) b.gameObject.SetActive(false);
        _continueButton.SetActive(true);
    }

    public void PressContinue()
    {
        OverworldUIManager.i.CloseSpecialEvent(); 
        foreach (var o in _decidedOutcomes) o.Trigger();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
