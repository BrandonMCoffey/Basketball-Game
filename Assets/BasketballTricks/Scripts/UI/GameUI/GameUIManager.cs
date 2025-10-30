using UnityEngine;
using System.Collections.Generic;


public enum TutorialState
{
    Intro,
    Catalog,
    DragPlayer,
    SelectPlayer,
    RemoveAction,
    SelectOtherPlayer,
    Simulate
}
public class GameUIManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool _startTutorial = true;

    [Header("Sliding Panels")]
    [SerializeField] SlideInPanel _cardCatalog;
    [SerializeField] SlideInPanel _timelinePanel;
    [SerializeField] SlideInPanel _playerActionPanel;

    [Header("TutorialPanels")]
    [SerializeField] RectTransform _tutorialBlocker;
    [SerializeField] TutorialState _currentTutorialState = TutorialState.Intro;
    [SerializeField] List<RectTransform> _tutorialPanels = new List<RectTransform>();
    void Start()
    {
        if (_startTutorial && _tutorialPanels.Count > 0)
        {
            _tutorialBlocker.gameObject.SetActive(true);
            foreach (var panel in _tutorialPanels)
            {
                panel.gameObject.SetActive(false);
            }

            _tutorialPanels[0].gameObject.SetActive(true);
        }
        else
        {
            foreach (var panel in _tutorialPanels)
            {
                panel.gameObject.SetActive(false);
            }
            _tutorialBlocker.gameObject.SetActive(false);

            _cardCatalog.SetShown(true);
            _timelinePanel.SetShown(true);
        }
    }

    public void NextTutorialState()
    {
        _currentTutorialState++;
        switch (_currentTutorialState)
        {
            case TutorialState.Catalog:
                _tutorialPanels[0].gameObject.SetActive(false);
                _tutorialPanels[1].gameObject.SetActive(true);
                _cardCatalog.transform.SetAsLastSibling();
                _cardCatalog.SetShown(true);
                break;
            case TutorialState.DragPlayer:
                _tutorialBlocker.gameObject.SetActive(false);
                _tutorialPanels[1].gameObject.SetActive(false);
                _tutorialPanels[2].gameObject.SetActive(true);
                break;
            case TutorialState.SelectPlayer:
                _cardCatalog.SetShown(false);
                _timelinePanel.SetShown(true);
                _tutorialPanels[2].gameObject.SetActive(false);
                _tutorialPanels[3].gameObject.SetActive(true);
                _playerActionPanel.transform.SetAsLastSibling();
                break;
            case TutorialState.RemoveAction:
                break;
            case TutorialState.SelectOtherPlayer:
                break;
            case TutorialState.Simulate:
                break;
        }
    }
}
