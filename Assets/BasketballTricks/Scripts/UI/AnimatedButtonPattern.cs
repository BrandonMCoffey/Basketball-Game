using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedButtonPattern : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private List<Image> _images;
    [SerializeField] private float _animationSpeed = 1f;
    [SerializeField] private float _delay = 1f;
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _highlightColor = Color.gray;
    [SerializeField] private Color _disabledColor = new Color(0, 0, 0, 0.5f);

    private bool _interactable = true;
    private float _animationTimer = 0f;
    private int _currentImageIndex = 0;

    private void OnValidate()
    {
        if (_button == null) _button = GetComponent<Button>();
        if (_images.Count == 0) _images = transform.GetComponentsInChildren<Image>(true).ToList();
    }

    private void Start()
    {

    }

    private void Update()
    {
        bool interactable = _button != null ? _button.interactable : true;
        if (interactable != _interactable)
        {
            _interactable = interactable;
            foreach (var img in _images)
            {
                img.color = _interactable ? _defaultColor : _disabledColor;
            }
            _animationTimer = 0f;
            _currentImageIndex = 0;
        }
        if (_interactable)
        {
            _animationTimer += Time.deltaTime * _animationSpeed * _images.Count;
            if (_animationTimer >= 1)
            {
                _animationTimer -= 1f;
                _currentImageIndex++;
                if (_currentImageIndex >= _images.Count)
                {
                    _currentImageIndex = -1;
                    _animationTimer -= _delay * (_images.Count - 1);
                }
                for (int i = 0; i < _images.Count; i++)
                {
                    _images[i].color = i == _currentImageIndex ? _highlightColor : _defaultColor;
                }
            }
        }
    }
}
