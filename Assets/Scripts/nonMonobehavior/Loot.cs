using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;



public class Loot
{
    public int Gold = 0;
    public EquipmentData Equipment = null;

    public static Loot GetLoot(float difficulty) {
        return GetLoot((int)difficulty);
    }

    public static Loot GetLoot(int difficulty) {
        Loot res = new Loot();

        float r = UnityEngine.Random.Range(0f, 1f);
        if (r < 0.1) {
            res.Equipment = EquipmentData.GetEquipment(difficulty);
        } else {
            res.Gold = 22 + UnityEngine.Random.Range(-5, 5);
        }
        
        return res;
    }
}
