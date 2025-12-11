using UnityEngine;
using DG.Tweening;
using UnityEditor.EditorTools;

public class GlareController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _glareTransform;
    
    [Header("Glare Settings")]
    [Tooltip("Vertical offset from screen edges where the glare starts and ends.")]
    [SerializeField] private float _verticalOffset = 500f;
    [SerializeField] private float _duration = 5f;
    [SerializeField] private bool _moveFromBottomToTop = false;

    void Start() 
    {
        float startY;
        float endY;

        if (_moveFromBottomToTop)
        {
            startY = -_verticalOffset;
            endY = Screen.height + _verticalOffset;
        }
        else
        {
            startY = Screen.height + _verticalOffset;
            endY = -_verticalOffset;
        }

        _glareTransform.anchoredPosition = new Vector2(_glareTransform.anchoredPosition.x, startY);
        _glareTransform.DOAnchorPosY(endY, _duration).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);    
    }
}
