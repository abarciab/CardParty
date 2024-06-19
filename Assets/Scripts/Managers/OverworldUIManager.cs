using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OverworldUIManager : UIManager
{
    public new static OverworldUIManager i;
    protected override void Awake() { base.Awake(); i = this; }

    [SerializeField] private Animator _wipe;
    [SerializeField] private SpecialEventController _eventController;

    [Header("TEMP")]
    [SerializeField, DisplayInspector] private List<SpecialEventData> _specialEvents = new List<SpecialEventData>();

    public async Task WipeScreen(float duration)
    {
        _wipe.gameObject.SetActive(true);
        await Task.Delay((int)(duration * 1000)); 
        _wipe.SetTrigger("exit");
    }

    public void StartRandomEvent()
    {
        OpenMenus += 1;
        _eventController.ShowEvent(_specialEvents[Random.Range(0, _specialEvents.Count)]);
    }

    public void CloseSpecialEvent()
    {
        OpenMenus -= 1;
        _eventController.Close();
    }
 }
