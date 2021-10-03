using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;
    event System.Action<SuitType, InputAction.CallbackContext> _onAction;
    event System.Action<InputAction.CallbackContext> _onInput;
    public System.Action<SuitType, InputAction.CallbackContext> OnAction { get => _onAction; set => _onAction = value; }

    public void AddListener(System.Action<InputAction.CallbackContext> action)
    {
        _onInput += action;
    }
    public void RemoveListener(System.Action<InputAction.CallbackContext> action)
    {
        _onInput -= action;
    }

    public void AddGameplayListener(System.Action<SuitType, InputAction.CallbackContext> action)
    {
        _onAction += action;
    }

    public void RemoveGameplayListener(System.Action<SuitType, InputAction.CallbackContext> action)
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
            default:
                _onInput?.Invoke(context);
                break;
        }
    }
}
