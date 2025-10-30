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
        _slideInPanel.ShowPanel();
        yield return new WaitForSecondsRealtime(0.2f);
        _timelinePanel.ShowPanel();
    }

    private void SetInitialOffScreenPositions()
    {
        _slideInPanel.Panel.anchoredPosition = _slideInPanel.OffScreenPosition;
        _timelinePanel.Panel.anchoredPosition = _timelinePanel.OffScreenPosition;
    }
}
