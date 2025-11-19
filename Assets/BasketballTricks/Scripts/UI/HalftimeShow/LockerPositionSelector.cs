using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockerPositionSelector : MonoBehaviour
{
    [SerializeField] private PlayerPosition _position;
    [SerializeField] private TMP_Text _positionText;
    [SerializeField] private CanvasGroup _selectPlayerPanel;
    [SerializeField] private CanvasGroup _playerInfoPanel;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private Image _playerImage;
    [SerializeField] private GameObject _naturalPosition;
    [SerializeField] private GameObject _actionPanel;
    [SerializeField] private List<TMP_Text> _actionList;

    private void OnValidate()
    {
        if (_positionText != null)
        {
            _positionText.text = _position switch
            {
                PlayerPosition.PointGuard => "PG",
                PlayerPosition.ShootingGuard => "SG",
                PlayerPosition.SmallForward => "SF",
                PlayerPosition.PowerForward => "PF",
                PlayerPosition.Center => "C",
                _ => "NA"
            };
        }
    }
}
