// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Events;
// //how it would work without this controller
// unityEvent.AddListener(triggeredEffect);

// public class TriggeredEffectController : MonoBehaviour
// {
//     // makes triggered effects trigger in the order they were added
//     private Dictionary<UnityEvent, List<ref TriggeredEffect>> _effectDict = new Dictionary<UnityEvent, List<TriggeredEffect>>();

//     public void AddEffect(UnityEvent unityEvent, ref TriggeredEffect triggeredEffect) {
//         if (!_effectDict.Keys.Contains(unityEvent)) AddEvent(unityEvent);
//         _effectDict.Add(unityEvent, triggeredEffect);
//     }

//     private void AddEvent(UnityEvent unityEvent) {
//         unityEvent.AddListener(TriggerEffects(unityEvent));
//     }

//     private void TriggerEffects(UnityEvent unityEvent) {
//         foreach (TriggeredEffect triggeredEffect in _effectDict[unityEvent]) {
//             triggeredEffect.TriggerEffect();
//         }
//     }
// }