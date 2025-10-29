using UnityEngine;
using DG.Tweening;
using System.Collections;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] MenuReference _slideInPanel;
    [SerializeField] MenuReference _timelinePanel;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        SetInitialOffScreenPositions();
        
        yield return new WaitForSecondsRealtime(0.2f);
        SlideInPanel();
        yield return new WaitForSecondsRealtime(0.2f);
        TimelinePanel();
    }

    private void SetInitialOffScreenPositions()
    {
        _slideInPanel.Panel.anchoredPosition = _slideInPanel.OffScreenPosition;
        _timelinePanel.Panel.anchoredPosition = _timelinePanel.OffScreenPosition;
    }

    private void SlideInPanel()
    {
        _slideInPanel.Panel.anchoredPosition = _slideInPanel.OffScreenPosition;
        _slideInPanel.Panel.DOAnchorPos(_slideInPanel.OnScreenPosition, _slideInPanel.SlideInDuration)
            .SetEase(_slideInPanel.SlideInEase);
    }

    private void TimelinePanel()
    {
        _timelinePanel.Panel.anchoredPosition = _timelinePanel.OffScreenPosition;
        _timelinePanel.Panel.DOAnchorPos(_timelinePanel.OnScreenPosition, _timelinePanel.SlideInDuration)
            .SetEase(_timelinePanel.SlideInEase);
    }
}
