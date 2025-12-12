using UnityEngine;
using DG.Tweening;
using System.Collections;

public class GlareController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _glareTransform;
    
    [Header("Glare Settings")]
    [Tooltip("Vertical offset from screen edges where the glare starts and ends.")]
    [SerializeField] private float _verticalOffset = 500f;
    [SerializeField] private float _duration = 5f;
    [SerializeField] private bool _moveFromBottomToTop = false;
    [SerializeField] private bool _randomizeStartDelay = false;
 
    IEnumerator Start() 
    {
        float startY;
        float endY;

        if (_moveFromBottomToTop)
        {
            startY = -_verticalOffset;
            Debug.Log("StartY: " + startY);
            endY = Screen.height + _verticalOffset;
        }
        else
        {
            startY = Screen.height + _verticalOffset;
            endY = -_verticalOffset;
        }

        if (_randomizeStartDelay)
        {
            float randomDelay = Random.Range(0f, 3f);
            yield return new WaitForSeconds(randomDelay);
        }

        _glareTransform.anchoredPosition = new Vector2(_glareTransform.anchoredPosition.x, startY);
        _glareTransform.DOAnchorPosY(endY, _duration).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);   

        yield return null; 
    }
}
