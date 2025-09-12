using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameChangerDisplay : DraggableContainer
{
    [SerializeField] private GameObject _gameChangerPrefab;
    [SerializeField] private float _spacing = 200f;

    public void AddGameChanger(GameChanger changerData)
    {
        GameObject newChangerObj = Instantiate(_gameChangerPrefab, transform);
        GameChangerUI changerUI = newChangerObj.GetComponent<GameChangerUI>();
        changerUI.Initialize(changerData, this);
        changerData.Initialize(changerUI);
        _items.Add(changerUI);
        UpdateLayout(false);
    }

    public List<GameChangerUI> GetOrderedChangers()
    {
        return _items.Cast<GameChangerUI>().ToList();
    }

    protected override void UpdateLayout(bool animated)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            float xPos = (i - (_items.Count - 1) / 2.0f) * _spacing;
            Vector3 targetPos = new Vector3(xPos, 0, 0);

            if (animated)
            {
                _items[i].transform.DOLocalMove(targetPos, 0.3f).SetEase(Ease.OutBack);
            }
            else
            {
                _items[i].transform.localPosition = targetPos;
            }
        }
    }
}