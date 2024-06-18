using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardGameManager : GameManager
{
    public new static CardGameManager i;
    protected override void Awake() { base.Awake(); i = this; }

    public void LoadOverworld()
    {
        Resume();
        StartCoroutine(FadeThenShowOverworld());
    }

    private IEnumerator FadeThenShowOverworld()
    {
        float fadeTime = UIManager.i.GetFadeTime();
        Music.FadeOutCurrent(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        Camera.GetComponent<AudioListener>().enabled = false;
        var unloadingTask = SceneManager.UnloadSceneAsync(2);
        //while (!unloadingTask.isDone) yield return null;

        if (OverworldManager.i) OverworldManager.i.ShowOverworldObjects();
        else SceneManager.LoadScene(1);
    }
}
