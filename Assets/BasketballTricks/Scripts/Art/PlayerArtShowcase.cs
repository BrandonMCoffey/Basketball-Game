using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerArtShowcase : MonoBehaviour
{
    [SerializeField] private List<PlayerData> _playerDatas;
    [SerializeField] private List<PlayerData> _playerDatas2;

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
        for (int i = 0; i < _playerDatas.Count; i++)
        {
            if (refIndex < _playerRefs.Count && _playerDatas[i] != null && _playerDatas[i].HasArtData)
            {
                _playerRefs[refIndex].gameObject.SetActive(true);
                _playerRefs[refIndex].PlayerArt.SetPlayerArt(_playerDatas[i].ArtData);
                _playerRefs[refIndex].SetPositionColor(Color.green);
                _playerRefs[refIndex].SetActionText(_playerDatas[i].PlayerName.Replace(" ", "\n"), -1);
                refIndex++;
            }
        }
        for (int i = 0; i < _playerDatas2.Count; i++)
        {
            if (refIndex < _playerRefs.Count && _playerDatas2[i] != null && _playerDatas2[i].HasArtData)
            {
                _playerRefs[refIndex].gameObject.SetActive(true);
                _playerRefs[refIndex].PlayerArt.SetPlayerArt(_playerDatas2[i].ArtData);
                _playerRefs[refIndex].SetPositionColor(Color.cyan);
                _playerRefs[refIndex].SetActionText(_playerDatas2[i].PlayerName.Replace(" ", "\n"), -1);
                refIndex++;
            }
        }
        for (int i = refIndex; i < _playerRefs.Count; i++)
        {
            _playerRefs[i].gameObject.SetActive(false);
            _playerRefs[refIndex].SetPositionColor(Color.red);
        }
    }
}
