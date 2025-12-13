using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BasketballPropertyModifier : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("How long it takes to fade from the Active Value back to the Reset Value.")]
    [SerializeField] private float _duration = 0.5f;

    [Tooltip("The value to start at immediately when triggered (e.g. high intensity).")]
    [SerializeField] private float _activeValue = 2.0f;

    [Tooltip("The value to fade towards (e.g. normal intensity).")]
    [SerializeField] private float _resetValue = 0.0f;

    [Space(10)]
    [Tooltip("Drag the target object here (e.g. Light) and select a Dynamic Float property.")]
    [SerializeField] private FloatEvent _targetProperty;

    private Coroutine _activeCoroutine;

    /// <summary>
    /// Call this method from the Basketball's "Event On Bounce" in the Inspector.
    /// </summary>
    public void TriggerEffect()
    {
        if (_activeCoroutine != null)
        {
            StopCoroutine(_activeCoroutine);
        }
        _activeCoroutine = StartCoroutine(LerpRoutine());
    }

    private IEnumerator LerpRoutine()
    {
        float elapsedTime = 0f;

        // Loop until the duration is reached
        while (elapsedTime < _duration)
        {
            // Calculate how far along we are (from 0.0 to 1.0)
            float t = elapsedTime / _duration;

            // Calculate the current value based on 't'
            // This fades FROM Active TO Reset
            float currentValue = Mathf.Lerp(_activeValue, _resetValue, t);

            // Apply the value
            _targetProperty?.Invoke(currentValue);

            // Wait for the next frame
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        // Ensure we land exactly on the reset value at the end
        _targetProperty?.Invoke(_resetValue);
        _activeCoroutine = null;
    }

    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }
}