using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField] int _maxPoints = 15;
    [SerializeField] TextMeshProUGUI _pointsValue;
    int _points;
    private void Start()
    {
        _points = _maxPoints;
        _pointsValue.text = _points.ToString();
    }
    public void AddPoints(int points)
    {
        _points = Mathf.Clamp(_points + points, 0, _maxPoints);
        _pointsValue.text = _points.ToString();
    }
}
