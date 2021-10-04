using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayEnd : MonoBehaviour {
    [SerializeField] GameObject _box = null;
    [SerializeField] Text _text = null;
    [SerializeField] Color _good = Color.white;
    [SerializeField] Color _bad = Color.white;
    
    void Start() {
        GameplayManager gameplayManager = FindObjectOfType<GameplayManager>();
        if (gameplayManager != null) {
            gameplayManager.OnStageComplete += GameFinished;
        }

        _box.SetActive(false);
    }

    void GameFinished(bool p_won) {
        if (p_won) {
            _text.text = "Thanks to you and to the power of music, humanity was saved from chaos and destruction.";
            _text.color = _good;
        }
        else {
            _text.text = "Thanks to your lack of musical sensitivity, the world will be destroyed";
            _text.color = _bad;
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
