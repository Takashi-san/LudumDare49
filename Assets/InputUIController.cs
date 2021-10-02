using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUIController : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;

    private void OnValidate()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetPress(bool pressed)
    {
        if (pressed)
        {
            _spriteRenderer.color = Color.green;
        }
        else
        {
            _spriteRenderer.color = Color.white;
        }
    }
}
