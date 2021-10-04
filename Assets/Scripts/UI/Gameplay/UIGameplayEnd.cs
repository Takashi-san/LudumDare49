using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayEnd : MonoBehaviour {
    [SerializeField] GameObject _box = null;
    [SerializeField] Text _text = null;
    
    void Start() {
        GameplayManager gameplayManager = FindObjectOfType<GameplayManager>();
        if (gameplayManager != null) {
            gameplayManager.OnStageComplete += GameFinished;
        }

        _box.SetActive(false);
    }

    void GameFinished(bool p_won) {
        if (p_won) {
            _text.text = "Magnificent!";
        }
        else {
            _text.text = "Desastrous!";
        }
        _box.SetActive(true);
    }

    public void ContinueButton() {
        GameManager.Instance.LoadScene("MainMenu");
    }

    public void RetryButton() {
        GameManager.Instance.ReloadScene();
    }
}
