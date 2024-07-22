using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class TriggeredEffectController : MonoBehaviour
{
    private Dictionary<TriggeredEffectTriggerTime, List<TriggeredEffect>> _triggeredEffects = new Dictionary<TriggeredEffectTriggerTime, List<TriggeredEffect>>();
    void Start() {
        CardGameManager.i.OnStartPlayerTurn.AddListener(delegate{TriggerEffects(TriggeredEffectTriggerTime.STARTOFPLAYERTURN);});
        CardGameManager.i.OnStartEnemyTurn.AddListener(delegate{TriggerEffects(TriggeredEffectTriggerTime.STARTOFENEMYTURN);});
        CardGameManager.i.OnEndPlayerTurn.AddListener(delegate{TriggerEffects(TriggeredEffectTriggerTime.ENDOFPLAYERTURN);});
        CardGameManager.i.OnEndEnemyTurn.AddListener(delegate{TriggerEffects(TriggeredEffectTriggerTime.ENDOFENEMYTURN);});
    }

    public void AddTriggeredEffect(TriggeredEffectData effectData) {
        print("adding triggered effect");
        TriggeredEffect newEffect = new TriggeredEffect(effectData);
        TriggeredEffectTriggerTime newTime = newEffect.TriggerTime;

        if (!_triggeredEffects.ContainsKey(newEffect.TriggerTime)) {
            _triggeredEffects.Add(newEffect.TriggerTime, new List<TriggeredEffect>());
        } else {
            for (int i = 0; i < _triggeredEffects[newTime].Count; i++) {
                if (newEffect.Type == _triggeredEffects[newTime][i].Type) {
                    _triggeredEffects[newTime][i] = _triggeredEffects[newTime][i] + newEffect;
                }
            }
        }
        _triggeredEffects[newTime].Add(newEffect);
        _triggeredEffects[newTime].OrderBy(x => (int)x.Type);
    }

    private void TriggerEffects(TriggeredEffectTriggerTime time) {
        if (!_triggeredEffects.ContainsKey(time)) {
            _triggeredEffects.Add(time, new List<TriggeredEffect>());
        }
        foreach (TriggeredEffect effect in _triggeredEffects[time]) {
            effect.Trigger();
        }
    }
}