using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum TutorialState
{
    Intro,
    Catalog,
    TapHoldPlayer,
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
                break;
            case TutorialState.TapHoldPlayer:
                break;
            case TutorialState.DragPlayer:
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
