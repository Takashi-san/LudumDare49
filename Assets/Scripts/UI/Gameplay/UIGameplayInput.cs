using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayInput : MonoBehaviour {
    [SerializeField] Image _image = null;

    [Header("A buttom")]
    [SerializeField] Sprite _spriteA = null;
    [SerializeField] Sprite _spriteHoldA = null;

    [Header("D buttom")]
    [SerializeField] Sprite _spriteD = null;
    [SerializeField] Sprite _spriteHoldD = null;

    [Header("Left buttom")]
    [SerializeField] Sprite _spriteLeft = null;
    [SerializeField] Sprite _spriteHoldLeft = null;

    [Header("Right buttom")]
    [SerializeField] Sprite _spriteRight = null;
    [SerializeField] Sprite _spriteHoldRight = null;

    SuitType _suitType;
    bool _isholding = false;
    Sprite _sprite;
    Sprite _spriteHold;

    public void Setup(SuitType p_suitType) {
        _suitType = p_suitType;
        SetSprite();
    }

    public void DoInput(bool p_isholding) {
        if (_isholding != p_isholding) {
            if (p_isholding) {
                _image.sprite = _spriteHold;
            }
            else {
                _image.sprite = _sprite;
            }
        }
        _isholding = p_isholding;
    }

    void SetSprite() {
        switch (_suitType) {
            case SuitType.Chord:
                _sprite = _spriteA;
                _spriteHold = _spriteHoldA;
                break;
            
            case SuitType.Metal:
                _sprite = _spriteD;
                _spriteHold = _spriteHoldD;
                break;
            
            case SuitType.Wood:
                _sprite = _spriteLeft;
                _spriteHold = _spriteHoldLeft;
                break;
            
            case SuitType.Percussion:
                _sprite = _spriteRight;
                _spriteHold = _spriteHoldRight;
                break;
        }
    }
}
