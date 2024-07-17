using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ShopData")]
public class ShopData : ScriptableObject
{
    [MaxValue(4), MinValue(0)] public int NumItems = 4;
    public List<Equipment> ItemOptions = new List<Equipment>();
    public List<AdventurerData> AdventurerOptions = new List<AdventurerData>();
    [ReadOnly] public bool AdventuerOptionUsed;

    public void initializeItemList()
    {
        var total = new List<Equipment>(ItemOptions.Where(x => !PlayerInfo.Inventory.Equipment.Contains(x)));
        ItemOptions.Clear();
        total = total.Shuffle().ToList();
        for (int i = 0; i < NumItems; i++) if (i < total.Count) ItemOptions.Add(total[i]);
    }
}
