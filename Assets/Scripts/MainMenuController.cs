using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Sound _clickSound;
    [SerializeField] private Fade _fade;
    [SerializeField] private MusicPlayer _music;

    private void Start()
    {
        _clickSound = Instantiate(_clickSound);
        _fade.Disappear();
    }

    public void Click()
    {
        _clickSound.Play();
    }

    public void StartGame()
    {
        StartCoroutine(TransitionToGame());
    }

    public void Quit()
    {
        Click();
        _music.FadeOutCurrent(_fade.FadeTime);
        _fade.Appear();
#if !UNITY_EDITOR
        Invoke(nameof(Application.Quit), _fade.FadeTime);
#endif
    }

    IEnumerator TransitionToGame()
    {
        _music.FadeOutCurrent(_fade.FadeTime);
        _fade.Appear();
        yield return new WaitForSeconds(_fade.FadeTime + 0.5f);
        Destroy(AudioManager.i.gameObject);
        SceneManager.LoadScene(1);
    }

}
