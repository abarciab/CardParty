using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OverworldUIManager : UIManager
{
    public new static OverworldUIManager i;
    protected override void Awake() { base.Awake(); i = this; }

    [SerializeField] private Animator _wipe;

    public async Task WipeScreen(float duration)
    {
        _wipe.gameObject.SetActive(true);
        await Task.Delay((int)(duration * 1000)); 
        _wipe.SetTrigger("exit");
    }
 }
