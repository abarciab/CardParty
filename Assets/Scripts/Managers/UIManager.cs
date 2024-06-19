using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager i;
    protected virtual void Awake() { i = this; }

    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Fade _fade;

    private void Start()
    {
        _fade.Disappear();
    }

    private string GetTimeString(int seconds)
    {
        seconds = Mathf.FloorToInt(seconds);
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        string timeString = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        return timeString;
    }

    public void SetPaused(bool isPaused)
    {
        _pauseMenu.SetActive(isPaused);
    }

    public void FadeToBlack()
    {
        _fade.Appear();
    }

    public void FadeFromBlack()
    {
        _fade.Disappear();
    }

    public float GetFadeTime() => _fade.FadeTime;
}
