using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenPanel : MonoBehaviour
{
    [SerializeField] Image _backgroundImage;
    [SerializeField] TMP_Text _playerNumberText;
    [SerializeField] TMP_Text _playerNameText;
    [SerializeField] TMP_Text _playerPositionText;
    [SerializeField] TMP_Text _hypeScoredText;
    [SerializeField] TMP_Text _xpGainedPos;

    bool _isInitialized = false;

    public void Init(string playerNumber, string playerName, string playerPos, int hypeScored, int xpGained)
    {
        if (_isInitialized) return;
        _isInitialized = true;
        _playerNumberText.text = playerNumber;
        _playerNameText.text = playerName;
        _playerPositionText.text = playerPos;
        _hypeScoredText.text = hypeScored.ToString();
        _xpGainedPos.text = xpGained.ToString();
    }
}
