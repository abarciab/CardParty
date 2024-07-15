using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageProp : MonoBehaviour
{
    [SerializeField] private List<Sprite> _sprites = new List<Sprite>();

    void Start()
    {
        transform.rotation = Quaternion.identity;
        foreach (var sRenderer in  GetComponentsInChildren<SpriteRenderer>()) sRenderer.sprite = _sprites[Random.Range(0, _sprites.Count)];
    }
}
