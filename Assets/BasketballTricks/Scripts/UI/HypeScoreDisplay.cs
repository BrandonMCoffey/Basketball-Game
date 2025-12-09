using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class HypeScoreDisplay : MonoBehaviour
{
    [SerializeField] private string _hypePrefix = "Hype: ";
    [SerializeField] private TMP_Text _hypeText;

    [SerializeField] float _animationDuration = 1f;

    private RectTransform _rectTransform;
    private float _currentAnchorPosY;
    int _currentHype;

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
        if (_hypeText != null)
        {
            _hypeText.transform.DOKill();
            _rectTransform.DOKill();

            _hypeText.transform.localScale = Vector3.one;
            _hypeText.transform.localRotation = Quaternion.identity;

            _animationDuration = Mathf.Max(1.0f, PlayerManager.Instance.CurrentActionDuration) * 0.75f;

            StopAllCoroutines();
            StartCoroutine(CountToScore(Mathf.CeilToInt(hype)));
        }
    }

    private IEnumerator CountToScore(int targetScore)
    {
        int startScore = _currentHype;
        float elapsed = 0f;
        
        int scoreDifference = targetScore - startScore;
        float minDiff = 10f;
        float maxDiff = 50f;
        float minScale = 1.1f;
        float maxScale = 1.75f;
        float minMove = 7f;
        float maxMove = 15f;

        float t = Mathf.InverseLerp(minDiff, maxDiff, scoreDifference);
        float targetScale = Mathf.Lerp(minScale, maxScale, t);

        float m = Mathf.InverseLerp(minDiff, maxDiff, scoreDifference);
        float targetMoveY = Mathf.Lerp(minMove, maxMove, m);

        _rectTransform.DOAnchorPosY(_currentAnchorPosY + targetMoveY, 0.1f).SetEase(Ease.OutQuad).SetDelay(_animationDuration).OnComplete(() =>
        {
            _rectTransform.DOAnchorPosY(_currentAnchorPosY, 0.2f).SetEase(Ease.OutBack).SetDelay(0.15f);
        });

        _hypeText.transform.DOScale(targetScale, 0.1f).SetEase(Ease.OutQuad).SetDelay(_animationDuration).OnComplete(() =>
        {
            _hypeText.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetDelay(0.15f);
        });

        while (elapsed < _animationDuration)
        {
            elapsed += Time.deltaTime;
            float t1 = elapsed / _animationDuration;

            t1 = 1f - Mathf.Pow(1f - t1, 2);

            float currentFloatScore = Mathf.Lerp(startScore, targetScore, t1);
            _currentHype = Mathf.RoundToInt(currentFloatScore);
            if (_hypeText != null) _hypeText.text = _hypePrefix + _currentHype.ToString();
            yield return null;
        }

        _currentHype = targetScore;
        if (_hypeText != null) _hypeText.text = _hypePrefix + _currentHype.ToString();
    }
}
