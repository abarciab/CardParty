using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreatureObjectUIController : MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _blockSlider;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _blockText;
    [SerializeField] private GameObject _labelParent;
    [SerializeField] private float _updateTime = 1;
    [SerializeField] private CopySlider _hpCopySlider;
    [SerializeField] private CopySlider _blockCopySlider;

    private CreatureObject _creatureObj;

    public void SetLabelVisible(bool visible) => _labelParent.SetActive(visible);
    private int _currentHealthValue;
    private int _currentBlockValue;

    private void OnEnable()
    {
        UpdateHealth(_creatureObj.HealthPercent);
        UpdateBlock(_creatureObj.BlockPercent);
    }

    public void Initialize(CreatureObject creatureObj)
    {
        _creatureObj = creatureObj;

        _hpSlider.value = 0.5f;
        _blockSlider.value = 0;
        _creatureObj.OnHealthPercentChanged.AddListener(UpdateHealth);
        _creatureObj.OnBlockPercentChanged.AddListener(UpdateBlock);

        _nameText.text = creatureObj.GetName();
    }

    public void UpdateHealth(float percent)
    {
        var health = _creatureObj.Health;
        if (gameObject.activeInHierarchy) StartCoroutine(UpdateVisuals(_hpText, _currentHealthValue, health, _hpSlider, percent * 0.5f, _hpCopySlider));
        _currentHealthValue = health;
    }

    public void UpdateBlock(float percent)
    {
        var block = _creatureObj.Block;
        if (gameObject.activeInHierarchy) StartCoroutine(UpdateVisuals(_blockText, _currentBlockValue, block, _blockSlider, percent * 0.5f, _blockCopySlider)) ;
        _currentBlockValue = block;
    }

    private IEnumerator UpdateVisuals(TextMeshProUGUI text, int currentValue, int targetValue, Slider slider, float targetSliderValue, CopySlider copySlider)
    {
        copySlider.SetFollower(slider.value);
        slider.value = targetSliderValue;

        float step = _updateTime / Mathf.Abs(targetValue - currentValue);
        float timePassed = step;
        while (timePassed < _updateTime) {
            var progress = timePassed / _updateTime;
            var incrementalValue = Mathf.Lerp(currentValue, targetValue, progress);
            text.text = Mathf.RoundToInt(incrementalValue).ToString();

            timePassed += step;
            yield return new WaitForSeconds(step);
        }

        text.text = targetValue.ToString();
        copySlider.UpdateFollowWithDelay(targetSliderValue);
    }
}
