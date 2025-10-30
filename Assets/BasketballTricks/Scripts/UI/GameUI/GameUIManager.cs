using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


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
    [SerializeField] SlideInPanel _submitPanel;

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
                _cardCatalog.SetShown(true, false);
                break;
            case TutorialState.DragPlayer:
                _tutorialBlocker.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                _tutorialPanels[1].gameObject.SetActive(false);
                _tutorialPanels[2].gameObject.SetActive(true);
                break;
            case TutorialState.SelectPlayer:
                _tutorialBlocker.gameObject.SetActive(false);
                _cardCatalog.SetShown(false, false);
                _timelinePanel.SetShown(true, false);
                _tutorialPanels[2].gameObject.SetActive(false);
                _tutorialPanels[3].gameObject.SetActive(true);
                _playerActionPanel.transform.SetAsLastSibling();
                _submitPanel.SetShown(false, false);
                break;
            case TutorialState.RemoveAction:
                _tutorialBlocker.gameObject.SetActive(true);
                _timelinePanel.transform.SetAsLastSibling();
                _playerActionPanel.SetShown(false, false);
                _submitPanel.SetShown(false, false);
                _tutorialPanels[3].gameObject.SetActive(false);
                _tutorialPanels[4].gameObject.SetActive(true);
                break;
            case TutorialState.SelectOtherPlayer:
                _tutorialBlocker.gameObject.SetActive(false);
                _tutorialPanels[4].gameObject.SetActive(false);
                _tutorialPanels[5].gameObject.SetActive(true);
                break;
            case TutorialState.Simulate:
                _playerActionPanel.SetShown(false, false);
                _submitPanel.SetShown(true, false);
                _tutorialPanels[5].gameObject.SetActive(false);
                _tutorialPanels[6].gameObject.SetActive(true);
                break;
        }
    }

    public void EndTutorial()
    {
        if (_startTutorial && _tutorialPanels.Count > 0)
        {
            foreach (var panel in _tutorialPanels)
            {
                panel.gameObject.SetActive(false);
            }
            _tutorialBlocker.gameObject.SetActive(false);
        }
    }
}
