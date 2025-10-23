using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    private int _screenWidth;
    private bool _isPortrait;

    private void OnValidate()
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        //UpdateSafeArea();
    }

    private void Awake()
    {
        UpdateSafeArea();
        _screenWidth = Screen.width;
        _isPortrait = Screen.height >= _screenWidth;
    }

    private void Update()
    {
        // Can this be done easier? Is there a UnityEvent for OnScreenRotate?
        if (_screenWidth != Screen.width)
        {
            _screenWidth = Screen.width;
            bool isPortrait = Screen.height >= _screenWidth;

            if (_isPortrait != isPortrait)
            {
                _isPortrait = isPortrait;
            }
            UpdateSafeArea();
        }
    }

    private void UpdateSafeArea()
    {
        if (_rectTransform == null) return;

        Rect safeArea = Screen.safeArea;

        Vector2 minAnchor = safeArea.position;
        Vector2 maxAnchor = safeArea.position + safeArea.size;

        _rectTransform.anchorMin = new Vector2(minAnchor.x / Screen.width, minAnchor.y / Screen.height);
        _rectTransform.anchorMax = new Vector2(maxAnchor.x / Screen.width, maxAnchor.y / Screen.height);

        _rectTransform.offsetMin = Vector2.zero;
        _rectTransform.offsetMax = Vector2.zero;
    }
}
