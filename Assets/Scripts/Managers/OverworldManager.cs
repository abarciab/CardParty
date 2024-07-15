using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InputController))]
public class OverworldManager : GameManager
{
    public static new OverworldManager i;
    protected override void Awake() { base.Awake(); i = this; }

    public OverworldPlayer Player;
    public CameraController CameraController;
    public TileGridController TileController;
    [SerializeField] private Transform _hider;

    private UIManager _UIManager;
    private AudioManager _AudioManager;

    [HideInInspector] public UnityEvent OnNewTileEntered;

    private void Start()
    {
        _UIManager = UIManager.i;
        _AudioManager = AudioManager.i;
    }

    public void ShowOverworldObjects()
    {
        if (_hider.childCount == 0) return;

        for (int i = _hider.childCount - 1; i >= 0; i--) {
            _hider.GetChild(i).SetParent(null);
        }
        gameObject.SetActive(true);
        RestoreSingletons();

        UIManager.i.FadeFromBlack();
        Player.enabled = true;
    }

    private void RestoreSingletons()
    {
        Awake();
        AudioManager.i = _AudioManager;
        UIManager.i = _UIManager;
    }

    public void LoadCardGame()
    {
        Player.enabled = false;
        UIManager.i.FadeToBlack();
        StartCoroutine(HideAndLoadCardGameAfterFade());
    }

    private IEnumerator HideAndLoadCardGameAfterFade()
    {
        yield return new WaitForSeconds(UIManager.i.GetFadeTime());
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
        HideAllOverworldObjects();
    }

    private void HideAllOverworldObjects()
    {
        var roots = SceneManager.GetSceneByBuildIndex(1).GetRootGameObjects();
        foreach (var r in roots) if (r != gameObject) r.transform.SetParent(_hider);
        gameObject.SetActive(false);
    }
}
