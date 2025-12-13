using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BasketballPropertyModifier : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("How long the property stays at the modified value.")]
    [SerializeField] private float _duration = 0.5f;

    [Tooltip("The value to apply immediately when triggered.")]
    [SerializeField] private float _activeValue = 2.0f;

    [Tooltip("The value to reset to after the duration ends.")]
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
        _activeCoroutine = StartCoroutine(Routine());
    }

    private IEnumerator Routine()
    {
        // 1. Apply Active Value
        _targetProperty?.Invoke(_activeValue);

        // 2. Wait
        yield return new WaitForSeconds(_duration);

        // 3. Apply Reset Value
        _targetProperty?.Invoke(_resetValue);

        _activeCoroutine = null;
    }

    // Serializable class to make the event appear in Inspector
    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }
}