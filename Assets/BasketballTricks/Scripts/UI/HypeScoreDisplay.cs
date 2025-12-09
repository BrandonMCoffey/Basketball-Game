using TMPro;
using UnityEngine;
using DG.Tweening;

public class HypeScoreDisplay : MonoBehaviour
{
    [SerializeField] private string _hypePrefix = "Hype: ";
    [SerializeField] private TMP_Text _hypeText;

    private RectTransform _rectTransform;
    private float _currentAnchorPosY;

    private void Awake() 
    {
        _rectTransform = GetComponent<RectTransform>();
        _currentAnchorPosY = _rectTransform.anchoredPosition.y;
    }

    private void OnEnable()
    {
        PlayerManager.UpdateHype += UpdateHype;
        if (PlayerManager.Instance != null) UpdateHype(PlayerManager.Instance.Hype);
    }

    private void Start()
    {
        if (PlayerManager.Instance != null) UpdateHype(PlayerManager.Instance.Hype);
    }

    private void OnDisable()
    {
        PlayerManager.UpdateHype -= UpdateHype;
    }

    private void UpdateHype(float hype)
    {
        if (_hypeText != null) _hypeText.text = _hypePrefix + Mathf.RoundToInt(hype).ToString();
        if (_hypeText != null)
        {
            _hypeText.transform.DOKill();
            _rectTransform.DOKill();

            _hypeText.transform.localScale = Vector3.one;
            _hypeText.transform.localRotation = Quaternion.identity;

            _rectTransform.DOAnchorPosY(5, 0.1f).SetEase(Ease.OutQuad);
            _rectTransform.DOScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutQuad);
            _hypeText.transform.DORotate(Vector3.one * Random.Range(-5f, 5f), 0.1f).SetEase(Ease.OutQuad);
            _hypeText.transform.DOScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                _rectTransform.DOAnchorPosY(_currentAnchorPosY, 0.2f).SetEase(Ease.OutBack);
                _rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
                _hypeText.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
                _hypeText.transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.OutBack);
            });
        }
    }
}
