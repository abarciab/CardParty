using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventCoordinator : MonoBehaviour
{
    [SerializeField] private List<UnityEvent> _events = new List<UnityEvent>();
    [SerializeField] private List<Sound> _sounds = new List<Sound>();

    private void Start()
    {
        for (int i = 0; i < _sounds.Count; i++) {
            _sounds[i] = Instantiate(_sounds[i]);
        }
    }

    public void Disable() => gameObject.SetActive(false);
    public void Destroy() => Destroy(gameObject);
    public void TriggerEvent1() => TEvent(0);
    public void TriggerEvent2() => TEvent(1);
    public void TriggerEvent3() => TEvent(2);
    public void PlaySound1() => PSound(0);
    public void PlaySound2() => PSound(1);
    public void PlaySound3() => PSound(2);

    private void TEvent(int index)
    {
        if (index < _events.Count) _events[index].Invoke();
    }

    private void PSound(int index)
    {
        if (index < _sounds.Count) _sounds[index].Play();
    }
}
