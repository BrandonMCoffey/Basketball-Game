using System.Collections.Generic;
using UnityEngine;

public class TopBarUIManager : MonoBehaviour
{
    [SerializeField] private TopBarActionUI _barActionPrefab;
    [SerializeField] private List<TopBarActionUI> _bars;

    private void Start()
    {
        PlayerManager.Instance.RefreshTimeline += UpdateTimeline;
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
        TopBarActionUI bar = Instantiate(_barActionPrefab, transform);
        _bars.Add(bar);
        return bar;
    }
}
