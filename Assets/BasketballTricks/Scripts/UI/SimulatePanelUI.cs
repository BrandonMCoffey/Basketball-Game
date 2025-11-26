using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimulatePanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _hypeText;
    [SerializeField] private List<SlideInPanel> _hideAllPanels;

    private void Awake()
    {
        _hypeText.text = "";
    }

    private void Start()
    {
        PlayerManager.UpdateHype += SetHype;
    }

    public void StartSimulate()
    {
        foreach (var panel in _hideAllPanels)
        {
            if (panel != null) panel.SetShown(false, false);
        }
        PlayerManager.Instance.RunSimulation();
    }

    public void SetHype(float hype)
    {
        if (_hypeText != null) _hypeText.text = $"Hype: {hype}";
    }
}
