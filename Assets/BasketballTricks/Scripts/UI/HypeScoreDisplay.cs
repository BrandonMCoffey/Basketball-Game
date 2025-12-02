using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HypeScoreDisplay : MonoBehaviour
{
    [SerializeField] private string _hypePrefix = "Hype: ";
    [SerializeField] private TMP_Text _hypeText;

    private void OnEnable()
    {
        PlayerManager.UpdateHype += UpdateHype;
        if (PlayerManager.Instance != null) UpdateHype(PlayerManager.Instance.Hype);
    }

    private void Start()
    {
        if (PlayerManager.Instance != null) UpdateHype(PlayerManager.Instance.Hype);
    }

    private void OnDisable()
    {
        PlayerManager.UpdateHype -= UpdateHype;
    }

    private void UpdateHype(float hype)
    {
        if (_hypeText != null) _hypeText.text = _hypePrefix + Mathf.RoundToInt(hype).ToString();
    }
}
