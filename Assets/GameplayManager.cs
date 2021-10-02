using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] BoardManager _boardManager;
    [SerializeField] InputManager _inputManager;

    private void OnEnable()
    {
        _inputManager.AddListener(OnAction);
    }

    private void OnDisable()
    {
        _inputManager.RemoveListener(OnAction);
    }

    void OnAction(ENaipes naipe, InputAction.CallbackContext context)
    {
        Debug.Log("OnAction");
        if (context.started)
            _boardManager.PressNaipe(naipe, true);
        else if(context.canceled)
            _boardManager.PressNaipe(naipe, false);
    }
}
