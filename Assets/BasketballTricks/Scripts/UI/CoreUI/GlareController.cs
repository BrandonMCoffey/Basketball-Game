using UnityEngine;
using DG.Tweening;

public class GlareController : MonoBehaviour
{
    [SerializeField] RectTransform _glareTransform;

    void Start() 
    {
        _glareTransform.anchoredPosition = new Vector2(_glareTransform.anchoredPosition.x, Screen.height + 500f);
        _glareTransform.DOAnchorPosY(-500f, 5f).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);    
    }
}
