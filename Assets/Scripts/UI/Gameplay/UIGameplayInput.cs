using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayInput : MonoBehaviour {
    [SerializeField] Image image = null;

    SuitType _suitType;
    bool _isholding = false;

    public void Setup(SuitType p_suitType) {
        _suitType = p_suitType;
    }

    public void DoInput(bool p_isholding) {
        if (_isholding != p_isholding) {
            image.color = Color.white - image.color;
        }
        _isholding = p_isholding;
    }
}
