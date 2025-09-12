using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameChangerUI : DraggableUIItem
{
    [SerializeField] private TextMeshProUGUI _nameText;
    public GameChanger Data { get; private set; }
    public bool IsDisabled { get; private set; }

    public void Initialize(GameChanger data, DraggableContainer parent)
    {
        Data = data;
        _nameText.text = data.Name;
        SetParentContainer(parent);
    }

    public void PlayEffectAnimation()
    {
        transform.DOShakePosition(0.4f, strength: 8, vibrato: 20).SetEase(Ease.OutCubic);
    }

    public void SetVisualsDisabled(bool isDisabled)
    {
        IsDisabled = isDisabled;
        _canvasGroup.alpha = isDisabled ? 0.5f : 1.0f;
    }
}