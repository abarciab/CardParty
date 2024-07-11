using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RandomizeSprite : MonoBehaviour
{
    [SerializeField] private List<Sprite> _sprites = new List<Sprite>();
    private Image _image;

    void Start()
    {
        _image = GetComponent<Image>();
        _image.sprite = _sprites[Random.Range(0, _sprites.Count)];
    }

}
