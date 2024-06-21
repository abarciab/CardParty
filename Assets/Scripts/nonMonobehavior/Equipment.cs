using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Equipment")]
public class Equipment : ScriptableObject
{
    public string Name;

    public override string ToString()
    {
        return Name;
    }
}
