using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIGameplayBackground : MonoBehaviour {
    [SerializeField] List<UIGameplayDestructableElement> _destructableElements = new List<UIGameplayDestructableElement>();

    void Start() {
        // TODO procurar gameplay manager e dar subscribe.
        // FindObjectOfType<GameplayManager>()?.lifeAction += UpdateLife;
    }

    void UpdateLife(int p_life) {
        int destructionStage = (11 - p_life) / 4;
        
        foreach (var element in _destructableElements) {
            element.UpdateDestructionStage(destructionStage);
        }
    }
}
