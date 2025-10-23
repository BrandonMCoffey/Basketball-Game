using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimulatePanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _pointsText;
    [SerializeField] private TMP_Text _multText;
    [SerializeField] private List<SlideInPanel> _hideAllPanels;

    private float _points;
    private float _mult;

    private void Awake()
    {
        _pointsText.text = "";
        _multText.text = "";
    }

    public void StartSimulate()
    {
        foreach (var panel in _hideAllPanels)
        {
            if (panel != null) panel.SetShown(false, false);
        }
        PlayerManager.Instance.RunSimulation(this);
    }

    public void ResetScore()
    {
        _points = 0;
        _mult = 0;
        _pointsText.text = $"Points: {_points}";
        _multText.text = $"Mult: {_mult}";
    }

    public void AddPoints(float points)
    {
        _points += points;
        _pointsText.text = $"Points: {_points}";
    }

    public void AddMult(float mult)
    {
        _mult += mult;
        _multText.text = $"Mult: {_mult}";
    }
}
