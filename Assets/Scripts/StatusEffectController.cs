using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class StatusEffectController : MonoBehaviour
{

    void Start() {
        CardGameManager.i.OnStartPlayerTurn.AddListener(delegate{TriggerAdventurerStatusEffects(StatusEffectTriggerTime.STARTOFTURN);});
        CardGameManager.i.OnStartEnemyTurn.AddListener(delegate{TriggerEnemyStatusEffects(StatusEffectTriggerTime.STARTOFTURN);});
        CardGameManager.i.OnEndPlayerTurn.AddListener(delegate{TriggerAdventurerStatusEffects(StatusEffectTriggerTime.ENDOFTURN);});
        CardGameManager.i.OnEndEnemyTurn.AddListener(delegate{TriggerEnemyStatusEffects(StatusEffectTriggerTime.ENDOFTURN);});
    }

    private void TriggerAdventurerStatusEffects(StatusEffectTriggerTime time) {
        foreach(AdventurerObject adventurer in CardGameManager.i.GetAdventurers()) {
            adventurer.TriggerStatusEffects(time);
        }
    }

    private void TriggerEnemyStatusEffects(StatusEffectTriggerTime time) {
        foreach(EnemyObject enemy in CardGameManager.i.GetEnemies()) {
            enemy.TriggerStatusEffects(time);
        }
    }
}
