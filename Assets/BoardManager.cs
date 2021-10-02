using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] List<InputUIController> _naipes;

    private void OnValidate()
    {
        _naipes = new List<InputUIController>(GetComponentsInChildren<InputUIController>());
    }
    public void PressNaipe(SuitType naipe, bool pressed)
    {
        Debug.Log("PressNaipe");
        _naipes[(int)naipe].SetPress(pressed);
    }
}
