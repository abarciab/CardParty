using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InputController))]
public class OverworldManager : GameManager
{
    public static new OverworldManager i;
    protected override void Awake() { base.Awake(); i = this; }

    public OverworldPlayer Player;
    public CameraController CameraController;

    public void LoadCardGame() => StartCoroutine(FadeThenLoadScene(2));
}
