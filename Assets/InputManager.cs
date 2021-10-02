using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;

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
                OnCorda(context);
                break;
            case "madeira":
                OnMadeira(context);
                break;
            case "metais":
                OnMetais(context);
                break;
            case "percussao":
                OnPercussao(context);
                break;
        }
    }

    void OnCorda(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("Corda pressed");
        if (context.canceled)
            Debug.Log("Corda released");
    }

    void OnMadeira(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("Madeira pressed");
        if (context.canceled)
            Debug.Log("Madeira released");
    }

    void OnMetais(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("Metais pressed");
        if (context.canceled)
            Debug.Log("Metais released");
    }

    void OnPercussao(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("Percussao pressed");
        if (context.canceled)
            Debug.Log("Percussao released");
    }
}
