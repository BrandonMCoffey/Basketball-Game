using UnityEngine;
using UnityEngine.Events;

public class Basketball : MonoBehaviour
{
    [SerializeField] private float _yThreshold = 0.1f;
    [SerializeField] private UnityEvent _eventOnBounce = new UnityEvent();

    private bool _grounded;

    private void Start()
    {
        _grounded = transform.position.y <= _yThreshold;
    }

    public void Update()
    {
        bool grounded = transform.position.y <= _yThreshold;
        if (grounded && !_grounded)
        {
            OnBounce();
        }
        _grounded = grounded;
    }

    private void OnBounce()
    {
        //SFXManager.PlaySfx(SFXEventEnum.BasketballBounce);
        _eventOnBounce?.Invoke();
    }
}
