using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TopBarUIManager : MonoBehaviour
{
    [SerializeField] private TopBarActionUI _barActionPrefab;
    [SerializeField] private RectTransform _barsContainer;
    [SerializeField] private List<TopBarActionUI> _bars;
    RectTransform _rectTransform;

    private void Start()
    {
        PlayerManager.Instance.RefreshTimeline += UpdateTimeline;
        _rectTransform = GetComponent<RectTransform>();
    }

    private void UpdateTimeline()
    {
        var timeline = PlayerManager.Instance.TimelineActions;
        int i = 0;
        for (; i < timeline.Count; i++)
        {
            var player = timeline[i].Player;
            var action = player.PlayerData.GetAction(timeline[i].ActionIndex);

            if (i < _bars.Count)
            {
                _bars[i].SetAction(player, action, i);
                _bars[i].gameObject.SetActive(true);
            }
            else
            {
                var newBar = AddTimelineBar();
                newBar.SetAction(player, action, i);
            }
        }
        for (; i < _bars.Count; i++)
        {
            _bars[i].gameObject.SetActive(false);
        }
    }

    private TopBarActionUI AddTimelineBar()
    {
        TopBarActionUI bar = Instantiate(_barActionPrefab, _barsContainer);
        RectTransform barRectTransform = bar.GetComponent<RectTransform>();
        barRectTransform.anchoredPosition = new Vector2(barRectTransform.anchoredPosition.x, barRectTransform.anchoredPosition.y + 50);
        _bars.Add(bar);

        barRectTransform.DOAnchorPosY(-40, 0.25f).SetEase(Ease.OutExpo);

        // _rectTransform.DOAnchorPosY(-100, 0.15f).SetEase(Ease.OutExpo)
        //     .OnComplete(() =>
        //     {
        //         _rectTransform.DOAnchorPosY(-80, 0.15f).SetEase(Ease.OutExpo);
        //     });
        return bar;
    }
}
