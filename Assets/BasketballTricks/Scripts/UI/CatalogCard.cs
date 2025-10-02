using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatalogCard : PlayerCard
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private GameObject _cardDisabled;
    [SerializeField] private Transform _cardVisuals;
    [SerializeField] private CanvasGroup _frontGroup;
    [SerializeField] private CanvasGroup _backGroup;

    private bool _isCardActive;
    private bool _isFlipped;

    public void InitCardActive(bool active)
    {
        _isCardActive = active;
        _toggle.isOn = active;
        _cardDisabled.SetActive(!active);
    }

    public void SetCardActive(bool active)
    {
        bool success = GameManager.Instance.SetPlayerActive(_index, active);
        if (success)
        {
            _isCardActive = active;
            _cardDisabled.SetActive(!active);
        }
        else
        {
            _toggle.isOn = _isCardActive;
        }
    }

    public void StartDribblePractice()
    {
        GameManager.Instance.StartDribblePractice(_index);
    }

    public void StartShootingPractice()
    {
        GameManager.Instance.StartShootingPractice(_index);
    }

    public void FlipCard()
    {
        StartCoroutine(FlipCardRoutine());
    }

    private IEnumerator FlipCardRoutine()
    {
        _isFlipped = !_isFlipped;
        bool halfway = false;
        _frontGroup.interactable = false;
        _frontGroup.blocksRaycasts = false;
        _backGroup.interactable = false;
        _backGroup.blocksRaycasts = false;
        for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
        {
            float delta = Mathf.Lerp(GameManager.EaseInOutQuart(t), t, 0.5f);
            float angle = _isFlipped ? Mathf.Lerp(0, 180, delta) : Mathf.Lerp(180, 360, delta);
            if (!halfway && angle >= (_isFlipped ? 90 : 270))
            {
                halfway = true;
                _frontGroup.alpha = _isFlipped ? 0 : 1;
                _backGroup.alpha = _isFlipped ? 1 : 0;
            }
            _cardVisuals.localRotation = Quaternion.Euler(0, angle, 0);
            _cardVisuals.localScale = Vector3.one + new Vector3(1f, 0.5f, 1f) * Mathf.Sin(t * Mathf.PI) * 0.1f;
            yield return null;
        }
        _frontGroup.interactable = !_isFlipped;
        _frontGroup.blocksRaycasts = !_isFlipped;
        _backGroup.interactable = _isFlipped;
        _backGroup.blocksRaycasts = _isFlipped;
    }
}
