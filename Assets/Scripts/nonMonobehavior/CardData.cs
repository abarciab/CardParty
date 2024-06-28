using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardData : ScriptableObject
{
    public CardObject cardObject = null;
    public Sprite cardGraphic;
    public IEnumerator currCardCoroutine;
    protected IEnumerator currSelectTargets;

    public virtual IEnumerator PlayCard() {
        if (currCardCoroutine == null) currCardCoroutine = PlayCard();
        yield return null;
    }

    public virtual IEnumerator CancelPlayCard() {
        yield return null;
    }
}