using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TutorialSection
{
    [TextArea(3, 10)] public string Text;
    public GameObject ToEnable;
    public bool IsSkipEnabled;

    public void Activate()
    {
        if (ToEnable) ToEnable.SetActive(true);
    }

    public void Deactivate()
    {
        if (ToEnable) ToEnable.SetActive(false);
    }
}

public class TutorialController : MonoBehaviour
{
    [SerializeField] private List<TutorialSection> _sections = new List<TutorialSection>();
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private Sound _nextSectionSound;

    private int _currentIndex;
    private TutorialSection _current => _currentIndex >= 0 && _currentIndex < _sections.Count ? _sections[_currentIndex] : null;

    private void Start()
    {
        _nextSectionSound = Instantiate(_nextSectionSound);
        StartTutorial();
    }

    private void StartTutorial()
    {
        _currentIndex = -1;
        //Time.timeScale = 0;
        //AudioManager.i.Pause();
        ShowNext();
    }

    private void Update()
    {
        if (_current.IsSkipEnabled && InputController.GetDown(Control.SKIP)) CompleteTutorial();
        if (InputController.GetDown(Control.NEXT)) ShowNext();
    }

    private void ShowNext()
    {
        _nextSectionSound.Play();
        _current?.Deactivate();
        _currentIndex += 1;
        if (_currentIndex >= _sections.Count) {
            CompleteTutorial();
            return;
        }
        _mainText.text = _current.Text;
        _current.Activate();
    }

    private void CompleteTutorial()
    {
        gameObject.SetActive(false);
        //Time.timeScale = 1;
        //AudioManager.i.Resume();
    }

}
