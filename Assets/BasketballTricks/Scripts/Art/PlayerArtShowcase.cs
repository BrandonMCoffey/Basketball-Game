using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerArtShowcase : MonoBehaviour
{
    [SerializeField] private List<PlayerData> _playerDatas;

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
            if (i < _playerDatas.Count && _playerDatas[i] != null && _playerDatas[i].HasArtData)
            {
                _playerRefs[refIndex].gameObject.SetActive(true);
                _playerRefs[refIndex].PlayerArt.SetPlayerArt(_playerDatas[i].ArtData);
                _playerRefs[refIndex].SetActionText(_playerDatas[i].PlayerName.Replace(" ", "\n"), -1);
                refIndex++;
            }
        }
        for (int i = refIndex; i < _playerRefs.Count; i++)
        {
            _playerRefs[i].gameObject.SetActive(false);
        }
    }
}
