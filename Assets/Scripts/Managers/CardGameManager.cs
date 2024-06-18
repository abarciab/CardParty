using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameManager : GameManager
{
    public new static CardGameManager i;
    protected override void Awake() { base.Awake(); i = this; }

    public void LoadOverworld()
    {
        Resume();
        StartCoroutine(FadeThenLoadScene(1));
    }
}
