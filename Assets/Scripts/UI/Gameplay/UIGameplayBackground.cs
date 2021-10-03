using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIGameplayBackground : MonoBehaviour {
    [SerializeField] List<UIGameplayDestructableElement> _destructableElements = new List<UIGameplayDestructableElement>();

    void Start() {
        GameplayManager gameplayManager = FindObjectOfType<GameplayManager>();
        if(gameplayManager != null)
            gameplayManager.OnLifeChange += UpdateLife;
    }

    void UpdateLife(int p_life) {
        int destructionStage = (11 - p_life) / 4;
        
        foreach (var element in _destructableElements) {
            element.UpdateDestructionStage(destructionStage);
        }
    }
}
