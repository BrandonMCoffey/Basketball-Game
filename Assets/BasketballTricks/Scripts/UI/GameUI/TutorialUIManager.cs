using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum TutorialState
{
    Intro,
    Catalog,
    DragPlayer,
    SelectPlayer,
    SelectAction,
    TimelineInfo,
    RemoveAction,
    SelectOtherPlayer,
    Simulate
}

public class TutorialUIManager : MonoBehaviour
{

    [Header("Slide Panel References")]
    [SerializeField] SlideInPanel _timelineBar;
    [SerializeField] SlideInPanel _cardContainer;
    [SerializeField] SlideInPanel _playerActionPanel;

    [Header("Tutorial Panels")]
    [SerializeField] RectTransform _backgroundDimmer;
    [SerializeField] RectTransform _introPanel;
    [SerializeField] RectTransform _catalogPanel;
    [SerializeField] RectTransform _dragPlayerPanel;

    [Header("Runtime Settings")]
    [SerializeField, ReadOnly] TutorialState _currentState = TutorialState.Intro;
    public TutorialState CurrentState => _currentState;

    public void NextState()
    {
        _currentState++;
        UpdateUIForState();
    }

    void UpdateUIForState()
    {
        switch (_currentState)
        {
            case TutorialState.Intro:
                break;
            case TutorialState.Catalog:
                _introPanel.gameObject.SetActive(false);
                _cardContainer.SetShown(true);
                _catalogPanel.gameObject.SetActive(true);
                break;
            case TutorialState.DragPlayer:
                _catalogPanel.gameObject.SetActive(false);
                _backgroundDimmer.gameObject.SetActive(false);
                _dragPlayerPanel.gameObject.SetActive(true);
                break;
            case TutorialState.SelectPlayer:
                break;
            case TutorialState.SelectAction:
                break;
            case TutorialState.TimelineInfo:
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
