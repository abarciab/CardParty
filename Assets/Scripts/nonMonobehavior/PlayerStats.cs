using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats 
{
    private int _money;
    public int Money { get { return _money; } set { _money = Mathf.Max(0, value); } }
}
