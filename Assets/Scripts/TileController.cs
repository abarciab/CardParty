using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField] private GameObject _interactable;

    private void Start()
    {
        if (Random.Range(0, 1f) < 0.5f) _interactable.SetActive(false);
    }

    public void ClickOnInteractable()
    {
        OverworldManager.i.LoadCardGame();
    }
}
