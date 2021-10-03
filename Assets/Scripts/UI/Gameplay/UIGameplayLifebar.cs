﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayLifebar : MonoBehaviour {
    [SerializeField] Image _image = null;
    [SerializeField] List<Sprite> _sprites = new List<Sprite>();

    void Start() {
        // TODO procurar gameplay manager e dar subscribe.
    }
    
    void UpdateLife(int p_life) {
        if (_sprites.Count != 11) {
            Debug.LogWarning("[UIGameplayLifebar] Incorrect amount of sprites!!");
            return;
        }
        int life = Mathf.Clamp(p_life, 0, 11);
        
        _image.sprite = _sprites[11 - life];
    }
}
