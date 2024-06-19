using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class SpecialEventChoiceData
{
    public string Text;
    [Range(0, 1)] public float SuccessChance;
    [SerializeField] private string _successText;

    [Header("Success")]
    [TextArea (3, 10) ] public string SuccessText;

    [Header("failure")]
    [TextArea(3, 10)] public string FailText;

    public string GetPercent()
    {
        return Mathf.FloorToInt(SuccessChance * 100) + "% " + _successText;
    }
}

[CreateAssetMenu(fileName ="new special event")]
public class SpecialEventData : ScriptableObject
{
    public string Title;
    [TextArea(3, 10)]public string Prompt;
    public Sprite Sprite;
    public int ChoicesToDisplay = 2;
    public List<SpecialEventChoiceData> Choices = new List<SpecialEventChoiceData>();
}
