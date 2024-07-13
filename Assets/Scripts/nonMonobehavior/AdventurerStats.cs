using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdventurerStats
{
    public int MaxHealth;

    private int _currentHealth;
    public int CurrentHealth { get { return _currentHealth; } set { SetHealth(value); } }

    private void SetHealth(int value) {
        _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }

    public AdventurerStats(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }
}
