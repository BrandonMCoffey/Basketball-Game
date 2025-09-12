using UnityEngine;
using TMPro;

public class DeckCountUI : MonoBehaviour
{
    private TextMeshProUGUI _deckCountText;

    private void Awake()
    {
        _deckCountText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        HandManager.OnDeckCountChanged += UpdateDeckCount;
    }

    private void OnDisable()
    {
        HandManager.OnDeckCountChanged -= UpdateDeckCount;
    }

    private void UpdateDeckCount(int count)
    {
        _deckCountText.text = $"Deck: {count}";
    }
}