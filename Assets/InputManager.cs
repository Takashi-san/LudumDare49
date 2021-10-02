using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;
    event System.Action<ENaipes, InputAction.CallbackContext> _onAction;

    public void AddListener(System.Action<ENaipes, InputAction.CallbackContext> action)
    {
        _onAction += action;
    }

    public void RemoveListener(System.Action<ENaipes, InputAction.CallbackContext> action)
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
                _onAction?.Invoke(ENaipes.CORDA, context);
                break;
            case "madeira":
                _onAction?.Invoke(ENaipes.MADEIRA, context);
                break;
            case "metais":
                _onAction?.Invoke(ENaipes.METAIS, context);
                break;
            case "percussao":
                _onAction?.Invoke(ENaipes.PERCUSSAO, context);
                break;
        }
    }
}
