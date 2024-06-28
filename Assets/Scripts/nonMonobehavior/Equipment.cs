using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Equipment")]
public class Equipment : ScriptableObject
{
    public string Name;
    public int Cost;

    public override string ToString()
    {
        return Name;
    }

    /*

    OnTurnStart(){
        if (!HasStartTurnBehavior) return;

        if (TURNSTARTBEHAV == HEALALLDUDES) HEALALLDUDES()
    }

    HEALALLDUDES() => GMan.healalldudes(amount)

    */
}
