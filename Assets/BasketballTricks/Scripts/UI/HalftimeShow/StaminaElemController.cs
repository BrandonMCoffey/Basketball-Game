using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public enum StaminaElemState
{
    Spent,
    Unused,
    Invisible
}
public class StaminaElemController : MonoBehaviour
{
    public StaminaElemState CurrentState = StaminaElemState.Invisible;
    private Color _unusedColor = Color.blue;
    private Color _spentColor = Color.green;
    
    [SerializeField] private Image _spriteRenderer;
    [SerializeField] private RectTransform _imageRectTransform;

    private CostDisplay _costDisplay;
    private bool _jumped = false;
    

    public void SetRefs(Color unusedColor, Color spentColor, CostDisplay costDisplay)
    {
        _unusedColor = unusedColor;
        _spentColor = spentColor;
        _costDisplay = costDisplay;
        UpdateVisuals();
    }

    private void UpdateVisuals(float delay = 0f)
    {
        switch (CurrentState)
        {
            case StaminaElemState.Spent:
                _spriteRenderer.enabled = true;
                Jump(delay);
                break;
            case StaminaElemState.Unused:
                _spriteRenderer.color = _unusedColor;
                _spriteRenderer.enabled = true;
                _imageRectTransform.DOAnchorPosX(0, 0.2f).SetEase(Ease.OutBack).SetDelay(delay);
                break;
            case StaminaElemState.Invisible:
                _spriteRenderer.enabled = false;
                _imageRectTransform.DOAnchorPosX(-100, 0.2f).SetEase(Ease.InBack).SetDelay(delay);
                break;
        }
    }

    private void Jump(float delay = 0f)
    {
        if (_jumped) return;
        _imageRectTransform.DOScale(Vector3.one * 1.5f, 0.1f).SetEase(Ease.OutQuad).SetDelay(delay);
        _imageRectTransform.DOAnchorPosY(20, 0.1f).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() =>
        {
            _spriteRenderer.color = _spentColor;
            _imageRectTransform.DOAnchorPosY(0, 0.2f).SetDelay(0.15f).SetEase(Ease.OutBack);
            _imageRectTransform.DOScale(Vector3.one, 0.2f).SetDelay(0.15f).SetEase(Ease.OutBack);

            var stamBarRect = _costDisplay.GetComponent<RectTransform>();
            stamBarRect.DOAnchorPosY(stamBarRect.anchoredPosition.y + 10, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                stamBarRect.DOAnchorPosY(stamBarRect.anchoredPosition.y - 10, 0.2f).SetEase(Ease.OutBack);
            });

        });
        _jumped = true;
    }

    public void Spend(float delay = 0f)
    {
        CurrentState = StaminaElemState.Spent;
        UpdateVisuals(delay);
    }

    public void Unuse()
    {
        CurrentState = StaminaElemState.Unused;
        _jumped = false;
        UpdateVisuals();
    }

    public void Hide()
    {
        CurrentState = StaminaElemState.Invisible;
        UpdateVisuals();
    }
}
