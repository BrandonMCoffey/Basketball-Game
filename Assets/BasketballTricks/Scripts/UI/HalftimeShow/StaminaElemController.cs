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

    

    public void SetColors(Color unusedColor, Color spentColor)
    {
        _unusedColor = unusedColor;
        _spentColor = spentColor;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        switch (CurrentState)
        {
            case StaminaElemState.Spent:
                _spriteRenderer.color = _spentColor;
                _spriteRenderer.enabled = true;
                _imageRectTransform.DOAnchorPosY(-10, 0.1f).SetEase(Ease.OutCirc).OnComplete(() =>
                {
                    _imageRectTransform.DOAnchorPosY(0, 0.2f).SetDelay(0.15f).SetEase(Ease.OutBack);
                });
                break;
            case StaminaElemState.Unused:
                _spriteRenderer.color = _unusedColor;
                _spriteRenderer.enabled = true;
                _imageRectTransform.DOAnchorPosX(0, 0.2f).SetEase(Ease.OutBack);
                break;
            case StaminaElemState.Invisible:
                _spriteRenderer.enabled = false;
                _imageRectTransform.DOAnchorPosX(-100, 0.2f).SetEase(Ease.InBack);
                break;
        }
    }

    public void Spend()
    {
        CurrentState = StaminaElemState.Spent;
        UpdateVisuals();
    }

    public void Unuse()
    {
        CurrentState = StaminaElemState.Unused;
        UpdateVisuals();
    }

    public void Hide()
    {
        CurrentState = StaminaElemState.Invisible;
        UpdateVisuals();
    }
}
