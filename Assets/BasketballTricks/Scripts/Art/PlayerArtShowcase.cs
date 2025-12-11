using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerArtShowcase : MonoBehaviour
{
    [SerializeField] private List<PlayerData> _playerDatasActive = new List<PlayerData>();
    [SerializeField] private List<PlayerData> _playerDatasUnused = new List<PlayerData>();
    [SerializeField] private List<PlayerData> _playerDatasNoArt = new List<PlayerData>();

    [Header("References")]
    [SerializeField] private Vector3 _spacing = Vector3.right;
    [SerializeField] private List<Player> _playerRefs;

    private void OnValidate()
    {
        _playerRefs = GetComponentsInChildren<Player>(true).ToList();
        for (int i = 0; i < _playerRefs.Count; i++)
        {
            _playerRefs[i].transform.localPosition = i * _spacing;
        }
        int refIndex = 0;
        void DisplayPlayer(PlayerData data, Color color)
        {
            if (refIndex < _playerRefs.Count && data != null)
            {
                _playerRefs[refIndex].gameObject.SetActive(true);
                _playerRefs[refIndex].PlayerArt.SetPlayerArt(data.ArtData);
                _playerRefs[refIndex].SetPositionIndicatorColor(color);
                _playerRefs[refIndex].SetActionText(data.PlayerName.Replace(" ", "\n"), -1);
                refIndex++;
            }
        }
        for (int i = 0; i < _playerDatasActive.Count; i++) DisplayPlayer(_playerDatasActive[i], Color.green);
        for (int i = 0; i < _playerDatasUnused.Count; i++) DisplayPlayer(_playerDatasUnused[i], Color.yellow);
        for (int i = 0; i < _playerDatasNoArt.Count; i++) DisplayPlayer(_playerDatasNoArt[i], Color.red);

        for (int i = refIndex; i < _playerRefs.Count; i++)
        {
            _playerRefs[i].gameObject.SetActive(false);
            _playerRefs[refIndex].SetPositionIndicatorColor(Color.red);
        }
    }
}
