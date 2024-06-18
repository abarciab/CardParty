using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseMenuController : MonoBehaviour
{
    [SerializeField] private Sound _clickSound;
    [SerializeField] private GameObject _settings;

    private void Start()
    {
        _clickSound = Instantiate(_clickSound);
        _settings.SetActive(false);
    }

    public void Resume()
    {
        GameManager.i.Resume();
        _clickSound.Play();
    }

    public void ExitToMainMenu()
    {
        GameManager.i.LoadMenu();
        _clickSound.Play();
    }

    public void ToggleSettings()
    {
        _settings.SetActive(!_settings.activeInHierarchy);
        Click();
    }

    public void Click() => _clickSound.Play();
}
