using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;
    event System.Action<SuitType, InputAction.CallbackContext> _onAction;

    public void AddListener(System.Action<SuitType, InputAction.CallbackContext> action)
    {
        _onAction += action;
    }

    public void RemoveListener(System.Action<SuitType, InputAction.CallbackContext> action)
    {
        _onAction -= action;
    }

    private void OnValidate()
    {
        _playerInput = GetComponent<PlayerInput>();
    }
    private void OnEnable()
    {
        _playerInput.onActionTriggered += OnActionTriggered;
    }

    private void OnDisable()
    {
        _playerInput.onActionTriggered -= OnActionTriggered;
    }

    void OnActionTriggered(InputAction.CallbackContext context)
    {
        Debug.Log($"Action triggered {context.action.name}");
        switch (context.action.name)
        {
            case "corda":
                _onAction?.Invoke(SuitType.Chord, context);
                break;
            case "madeira":
                _onAction?.Invoke(SuitType.Wood, context);
                break;
            case "metais":
                _onAction?.Invoke(SuitType.Metal, context);
                break;
            case "percussao":
                _onAction?.Invoke(SuitType.Percussion, context);
                break;
        }
    }
}
