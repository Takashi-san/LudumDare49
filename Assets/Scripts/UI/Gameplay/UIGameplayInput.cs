using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayInput : MonoBehaviour {
    [SerializeField] Image _image = null;

    [Header("Yellow buttom")]
    [SerializeField] Sprite _spriteYellow = null;
    [SerializeField] Sprite _spriteHoldYellow = null;

    [Header("Purple buttom")]
    [SerializeField] Sprite _spritePurple = null;
    [SerializeField] Sprite _spriteHoldPurple = null;

    [Header("Blue buttom")]
    [SerializeField] Sprite _spriteBlue = null;
    [SerializeField] Sprite _spriteHoldBlue = null;

    [Header("Green buttom")]
    [SerializeField] Sprite _spriteGreen = null;
    [SerializeField] Sprite _spriteHoldGreen = null;

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
                _sprite = _spriteYellow;
                _spriteHold = _spriteHoldYellow;
                break;
            
            case SuitType.Metal:
                _sprite = _spriteBlue;
                _spriteHold = _spriteHoldBlue;
                break;
            
            case SuitType.Wood:
                _sprite = _spritePurple;
                _spriteHold = _spriteHoldPurple;
                break;
            
            case SuitType.Percussion:
                _sprite = _spriteGreen;
                _spriteHold = _spriteHoldGreen;
                break;
        }
    }
}
