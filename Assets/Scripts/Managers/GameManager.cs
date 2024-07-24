using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager i;
    protected virtual void Awake() { i = this; }

    [SerializeField] protected MusicPlayer Music;
    public Transform Camera;
    private Camera _cam;

    [HideInInspector] public OnClickOnCollider CurrentHoveredOnCollider;

    protected virtual void Start()
    {
        _cam = Camera.GetComponentInChildren<Camera>();
    }

    protected virtual void Update()
    {
        if (InputController.GetDown(Control.PAUSE)) TogglePause();
        CheckForCurrentHovered();
    }

    private void CheckForCurrentHovered()
    {
        var mouseRay = _cam.ScreenPointToRay(Input.mousePosition);
        bool hitPoint = Physics.Raycast(mouseRay, out var hitData);
        if (!hitPoint) CurrentHoveredOnCollider = null;
        else {
            //print("new hovered: " + hitData.collider.gameObject.name);
            var OnClick = hitData.collider.GetComponentInParent<OnClickOnCollider>();
            CurrentHoveredOnCollider = OnClick;
        }
    }

    void TogglePause()
    {
        if (Time.timeScale == 0) Resume();
        else Pause();
    }

    public void Resume()
    {
        UIManager.i.SetPaused(false);
        Time.timeScale = 1;
        AudioManager.i.Resume();
    }

    public void Pause()
    {
        UIManager.i.SetPaused(true);
        Time.timeScale = 0;
        AudioManager.i.Pause();
    }

    [ButtonMethod]
    public void LoadMenu()
    {
        print("loading menu");
        Resume();
        StartCoroutine(FadeThenLoadScene(0));
    }

    [ButtonMethod]
    public void EndGame()
    {
        Resume();
        StartCoroutine(FadeThenLoadScene(3));
    }

    protected IEnumerator FadeThenLoadScene(int num)
    {
        UIManager.i.FadeToBlack();
        var fadeTime = UIManager.i.GetFadeTime();
        Music.FadeOutCurrent(fadeTime);
        yield return new WaitForSeconds(fadeTime + 0.5f);
        Destroy(AudioManager.i.gameObject);
        SceneManager.LoadScene(num);
    }
}