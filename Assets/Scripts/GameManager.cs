using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : SingletonMonobehaviour<GameManager>
{
    [SerializeField] string _gameplayScene;
    [SerializeField] string _mainmenuScene;
    [SerializeField] string _creditsScene;

    protected override void Awake()
    {
        base.Awake();
        if (!IsSingletonInstance()) {
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
