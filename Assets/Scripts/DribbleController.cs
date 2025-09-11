using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DribbleController : MonoBehaviour
{
    [SerializeField] private int _dribbles;
    [SerializeField] private TMP_Text _dribblesText;

    [Header("Dribble Forces")]
    [SerializeField] private float _swipeForce = 0.015f;
    [SerializeField] private float _groundLaunchForce = 3f;
    [SerializeField] private float _bounciness = 0.7f;
    [SerializeField] private float _tapForceMultiplier = 0.7f;
    [SerializeField] private Vector2 _forceRandomness = new Vector2(0.95f, 1.05f);
    [SerializeField] private float _distanceFromCenterToCrossover = 0.15f;
    [SerializeField] private float _crossoverForce = 1.5f;
    [SerializeField] private float _spinMultiplier = 250f;

    [Header("Input Settings")]
    [SerializeField] private float _maxSwipeForce = 10f;
    [SerializeField] private float _maxInputDistance = 300f;
    [SerializeField] private float _inputProcessTime = 0.1f;
    [SerializeField] private float _swipeThreshold = 50f;
    [SerializeField] private float _groundProximity = 0.1f;
    [SerializeField] private float _horizontalSwipeMultiplier = 0.5f;
    [SerializeField] private float _upwardsSwipeMultiplier = 0.5f;

    [Header("Boundary Control")]
    [SerializeField] private float _boundaryForce = 30f;
    [SerializeField] private Vector2 _landscapeBoundaryMargin = new Vector2(0.05f, 0.1f);
    [SerializeField] private Vector2 _portraitBoundaryMargin = new Vector2(0.1f, 0.05f);

    private Rigidbody _rb;
    private Camera _mainCamera;
    private SphereCollider _sphereCollider;
    private Vector2 _touchStartPos;
    private float _touchStartTime;
    private bool _isTouchValid;
    private float _ballRadius;
    private int _touches;

    private bool Grounded => transform.position.y <= _ballRadius + _groundProximity;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        _sphereCollider = GetComponent<SphereCollider>();
        _ballRadius = _sphereCollider.radius * transform.localScale.y;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Vector3 ballScreenPos = _mainCamera.WorldToScreenPoint(transform.position);
                float distance = Vector2.Distance(touch.position, ballScreenPos);

                if (distance <= _maxInputDistance)
                {
                    _isTouchValid = true;
                    _touchStartTime = Time.time;
                    _touchStartPos = touch.position;
                }
                else
                {
                    _isTouchValid = false;
                }
            }

            if (_isTouchValid && (Time.time - _touchStartTime >= _inputProcessTime || touch.phase == TouchPhase.Ended))
            {
                ProcessTouch(touch);
                _isTouchValid = false;
            }
        }
        if (Grounded && _touches > 0)
        {
            _dribbles += 1;
            if (_touches > 1) Debug.Log($"{_touches} touches before touching floor");
            _touches = 0;
            _dribblesText.text = $"Dribbles: {_dribbles}";
        }
    }

    private void FixedUpdate()
    {
        HandleBoundaries();
    }

    private void ProcessTouch(Touch touch)
    {
        float swipeDistance = (touch.position - _touchStartPos).magnitude;
        if (swipeDistance < _swipeThreshold)
        {
            HandleTap(touch);
        }
        else
        {
            HandleSwipe(touch);
        }
    }

    private void HandleTap(Touch touch)
    {
        if (Grounded)
        {
            _rb.AddForce(Vector3.up * _groundLaunchForce, ForceMode.Impulse);
            return;
        }

        Vector3 ballScreenPos = _mainCamera.WorldToScreenPoint(transform.position);

        float screenRadius = Vector3.Distance(ballScreenPos, _mainCamera.WorldToScreenPoint(transform.position + new Vector3(_sphereCollider.radius, 0, 0)));
        if (screenRadius > 0)
        {
            float horizontalDistance = Mathf.Abs(touch.position.x - ballScreenPos.x);
            float centeredness = 1.0f - Mathf.Clamp01(horizontalDistance / screenRadius);
            _rb.velocity = new Vector3(_rb.velocity.x * (1.0f - centeredness), _rb.velocity.y, _rb.velocity.z);
        }

        float height = transform.position.y;
        if (height <= 0) return;

        float requiredVelocity = Mathf.Sqrt(2 * Physics.gravity.magnitude * height);
        float verticalImpulse = _rb.mass * -requiredVelocity - _rb.velocity.y;

        verticalImpulse *= _tapForceMultiplier;
        verticalImpulse *= Random.Range(_forceRandomness.x, _forceRandomness.y);

        bool tappedRightSide = touch.position.x > ballScreenPos.x;
        float tapDirectionX = tappedRightSide ? -1.0f : 1.0f;
        float horizontalImpulse = tapDirectionX * _crossoverForce;

        Vector3 finalImpulse = new Vector3(horizontalImpulse, verticalImpulse, 0);
        Debug.Log($"Tap: {finalImpulse}");
        _rb.AddForce(finalImpulse, ForceMode.Impulse);
        _touches++;
    }

    private void HandleSwipe(Touch touch)
    {
        Vector2 swipeDelta = touch.position - _touchStartPos;
        if (swipeDelta.y > 0) swipeDelta.y *= _upwardsSwipeMultiplier;
        Vector3 forceVector = new Vector3(swipeDelta.x * _horizontalSwipeMultiplier, swipeDelta.y, 0) * _swipeForce;

        Debug.Log($"Swipe: {forceVector}");
        if (forceVector.magnitude > _maxSwipeForce)
        {
            forceVector = forceVector.normalized * _maxSwipeForce;
        }
        forceVector.y -= _rb.velocity.y;
        _rb.AddForce(forceVector, ForceMode.Impulse);

        Vector3 torqueAxis = new Vector3(swipeDelta.y, -swipeDelta.x, 0).normalized;
        _rb.AddTorque(torqueAxis * _spinMultiplier, ForceMode.Acceleration);
        _touches++;
    }

    private void HandleBoundaries()
    {
        Vector2 currentMargin = Screen.width > Screen.height ? _landscapeBoundaryMargin : _portraitBoundaryMargin;
        Vector3 viewportPos = _mainCamera.WorldToViewportPoint(transform.position);

        if (viewportPos.x < currentMargin.x)
        {
            float penetration = Mathf.InverseLerp(currentMargin.x, 0f, viewportPos.x);
            _rb.AddForce(Vector3.right * _boundaryForce * penetration);
        }
        else if (viewportPos.x > 1 - currentMargin.x)
        {
            float penetration = Mathf.InverseLerp(1 - currentMargin.x, 1f, viewportPos.x);
            _rb.AddForce(Vector3.left * _boundaryForce * penetration);
        }

        if (viewportPos.y > 1 - currentMargin.y)
        {
            float penetration = Mathf.InverseLerp(1 - currentMargin.y, 1f, viewportPos.y);
            _rb.AddForce(Vector3.down * _boundaryForce * penetration);
        }

        Vector3 currentVelocity = _rb.velocity;
        bool velocityChanged = false;

        if (viewportPos.x <= 0f && currentVelocity.x < 0)
        {
            currentVelocity.x = 0;
            velocityChanged = true;
        }
        else if (viewportPos.x >= 1f && currentVelocity.x > 0)
        {
            currentVelocity.x = 0;
            velocityChanged = true;
        }

        if (viewportPos.y >= 1f && currentVelocity.y > 0)
        {
            currentVelocity.y = 0;
            velocityChanged = true;
        }

        if (velocityChanged)
        {
            _rb.velocity = currentVelocity;
        }
    }
}