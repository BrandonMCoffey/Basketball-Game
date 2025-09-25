using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatalogCard : PlayerCard
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private GameObject _cardDisabled;

    private bool _isCardActive;

    public void InitCardActive(bool active)
    {
        _isCardActive = active;
        _toggle.isOn = active;
        _cardDisabled.SetActive(!active);
    }
    public void SetCardActive(bool active)
    {
        bool success = GameManager.Instance.SetPlayerActive(_index, active);
        if (success)
        {
            _isCardActive = active;
            _cardDisabled.SetActive(!active);
        }
        else
        {
            _toggle.isOn = _isCardActive;
        }
    }

    public void StartDribblePractice()
    {
        GameManager.Instance.StartDribblePractice(_index);
    }

    public void StartShootingPractice()
    {
        GameManager.Instance.StartShootingPractice(_index);
    }
}
