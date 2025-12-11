using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum TutorialChangeEvent
{
    None,
    CardPlayed,
    NextButtonPressed,
    GamePlayed,
    AfterSequenceEnd
}
public class TutorialPanelController : MonoBehaviour
{
    [SerializeField] TutorialChangeEvent _changeEvent = TutorialChangeEvent.NextButtonPressed;
    [SerializeField] int _numCardsToPlay = 1;
    [SerializeField] UnityEvent _OnPanelOpened;
    [SerializeField] UnityEvent _OnPanelClosed;
    [SerializeField] UnityEvent OnNext;

    [SerializeField] LongButtonController _nextButton;
    [SerializeField] TMP_Text _messageText;
    [SerializeField, TextArea(3, 5)] string _text;

    private void OnValidate() 
    {
        if (_messageText != null)
        {
            _messageText.text = _text;
        }

        if (_nextButton != null && _changeEvent == TutorialChangeEvent.NextButtonPressed)
        {
            _nextButton.gameObject.SetActive(true);
        }
        else if (_nextButton != null)
        {
            _nextButton.gameObject.SetActive(false);
        }
    }

    private void Start() 
    {
        _nextButton.OnButtonPressed += NextButtonPressed;   
    }

    private void OnEnable() 
    {
        ActionDeckManager.OnCardPlayed += CardPlayed;
        ActionDeckManager.OnPlayPressed += PlayPressed;
        ActionDeckManager.OnSequenceEnd += SequenceEnd;
        _OnPanelOpened?.Invoke();
    }

    private void OnDisable() 
    {
        ActionDeckManager.OnCardPlayed -= CardPlayed;
        ActionDeckManager.OnPlayPressed -= PlayPressed;
        ActionDeckManager.OnSequenceEnd -= SequenceEnd;
        _OnPanelClosed?.Invoke();
        _nextButton.OnButtonPressed -= NextButtonPressed;
    }

    private void NextButtonPressed()
    {
        if (_changeEvent != TutorialChangeEvent.NextButtonPressed) return;
        OnNext?.Invoke();
    }

    private void CardPlayed(ActionCard playedCard)
    {
        if (_changeEvent != TutorialChangeEvent.CardPlayed) return;
        _numCardsToPlay--;
        if (_numCardsToPlay <= 0)
        {
            OnNext?.Invoke();
        }
    }

    private void PlayPressed()
    {
        if (_changeEvent != TutorialChangeEvent.GamePlayed) return;
        OnNext?.Invoke();
    }

    private void SequenceEnd()
    {
        if (_changeEvent != TutorialChangeEvent.AfterSequenceEnd) return;
        OnNext?.Invoke();
    }
}
