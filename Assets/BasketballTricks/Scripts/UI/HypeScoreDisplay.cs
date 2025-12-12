using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class HypeScoreDisplay : MonoBehaviour
{
    [SerializeField] private string _hypePrefix = "Hype: ";
    [SerializeField] private TMP_Text _hypeText;
    [SerializeField] private bool _hypeTextAddAnim = true;
    [SerializeField, ShowIf("_hypeTextAddAnim")] private TMP_Text _hypeAddEffectText;
    [SerializeField] float _animationDuration = 1f;

    [Header("HypeBar")]
    [SerializeField] Slider _hypeBar;
    [SerializeField] RectTransform _goalTransform;
    [SerializeField] RectTransform _bronzeTransform;
    [SerializeField] RectTransform _silverTransform;
    [SerializeField] RectTransform _goldTransform;
    [SerializeField] RectTransform _fillArea;
    float _goalPos;
    float _bronzePos;
    float _silverPos;
    float _goldPos;

    private RectTransform _rectTransform;
    private float _currentAnchorPosY;
    int _currentHype;
    float _hypeAddYStartPos;

    Coroutine _fillBarRoutine;

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
        _hypeAddYStartPos = _hypeAddEffectText.rectTransform.anchoredPosition.y;
        _hypeAddEffectText.enabled = false;
    }

    private void OnDisable()
    {
        PlayerManager.UpdateHype -= UpdateHype;
    }

    public void Init(float goal, float bronze, float silver, float gold)
    {
        _goalPos = MapValue(goal, 0, ActionDeckManager.MaxScore, 0, 1);
        _bronzePos = MapValue(bronze, 0, ActionDeckManager.MaxScore, 0, 1);
        _silverPos = MapValue(silver, 0, ActionDeckManager.MaxScore, 0, 1);
        _goldPos = MapValue(gold, 0, ActionDeckManager.MaxScore, 0, 1);

        _hypeBar.value = 0f;

        _goalTransform.anchoredPosition = new Vector2(MapValue(goal, 0, ActionDeckManager.MaxScore, -160, 160), 200);  
        _bronzeTransform.anchoredPosition = new Vector2(MapValue(bronze, 0, ActionDeckManager.MaxScore, -160, 160), 200);  
        _silverTransform.anchoredPosition = new Vector2(MapValue(silver, 0, ActionDeckManager.MaxScore, -160, 160), 200);  
        _goldTransform.anchoredPosition = new Vector2(MapValue(gold, 0, ActionDeckManager.MaxScore, -160, 160), 200);    

        _goalTransform.DOAnchorPosY(0, 2f).SetEase(Ease.OutQuad);
        _bronzeTransform.DOAnchorPosY(0, 2f).SetEase(Ease.OutQuad).SetDelay(0.1f);
        _silverTransform.DOAnchorPosY(0, 2f).SetEase(Ease.OutQuad).SetDelay(0.15f);
        _goldTransform.DOAnchorPosY(0, 2f).SetEase(Ease.OutQuad).SetDelay(0.2f);  
    }

    float MapValue(float value, float from1, float to1, float from2, float to2) => (value - from1) / (to1 - from1) * (to2 - from2) + from2;

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

        if (_hypeBar != null)
        {
            if (_fillBarRoutine != null) StopCoroutine(_fillBarRoutine);
            _fillBarRoutine = StartCoroutine(FillBar(hype));
        }
    }

    private IEnumerator FillBar(float targetHype)
    {
        float startHype = _hypeBar.value;
        float targetHypeSliderVal = MapValue(targetHype, 0, ActionDeckManager.MaxScore, 0, 1);
        float elapsed = 0f;

        while (elapsed < _animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _animationDuration;

            t = 1f - Mathf.Pow(1f - t, 2);

            float currentHype = Mathf.Lerp(startHype, targetHypeSliderVal, t);
            _hypeBar.value = currentHype;

            if (_hypeBar.value >= _goldPos)
            {
                _goldTransform.DOScale(1.5f, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    _goldTransform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
                });
            }
            else if (_hypeBar.value >= _silverPos)
            {
                _silverTransform.DOScale(1.5f, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    _silverTransform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
                });
            }
            else if (_hypeBar.value >= _bronzePos)
            {
                _bronzeTransform.DOScale(1.5f, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    _bronzeTransform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
                });
            }
            else if (_hypeBar.value >= _goalPos)
            {
                _goalTransform.DOScale(1.5f, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    _goalTransform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
                });
            }

            yield return null;
        }

        _hypeBar.value = targetHypeSliderVal;
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

        if (_hypeTextAddAnim)
        {
            _hypeAddEffectText.enabled = true;
            _hypeAddEffectText.text = "+" + scoreDifference.ToString();
            _hypeAddEffectText.rectTransform.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.2f).OnComplete(() =>
            {
                _hypeAddEffectText.enabled = false;
                _hypeAddEffectText.rectTransform.anchoredPosition = new Vector2(_hypeAddEffectText.rectTransform.anchoredPosition.x, _hypeAddYStartPos);
            });
        }

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
