using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimulatePanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _pointsText;
    [SerializeField] private TMP_Text _multText;
    [SerializeField] private List<SlideInPanel> _hideAllPanels;

    public float Points { get; private set; }
    public float Mult { get; private set; }

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
        Points = 0;
        Mult = 1;
        _pointsText.text = $"Points: {Points}";
        _multText.text = $"Mult: {Mult}";
    }

    public void AddPoints(float points)
    {
        Points += points;
        _pointsText.text = $"Points: {Points}";
    }

    public void AddMult(float mult)
    {
        Mult += mult;
        _multText.text = $"Mult: {Mult}";
    }
}
