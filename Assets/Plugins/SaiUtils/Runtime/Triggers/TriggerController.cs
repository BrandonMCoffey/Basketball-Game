using System;
using SaiUtils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace SaiUtils.Triggers
{

    public enum TriggerEventType
    {
        Enter,
        Stay,
        Exit
    }
    [RequireComponent(typeof(Collider))]
    public class TriggerController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] LayerMask _desiredLayer;
        [SerializeField] Collider _collider;

        [Header("Events")]
        [SerializeField] UnityEvent<GameObject> _onTriggerEnter;
        [SerializeField] UnityEvent<GameObject> _onTriggerStay;
        [SerializeField] UnityEvent<GameObject> _onTriggerExit;
        public Action<GameObject> OnTriggerEnterAction;
        public Action<GameObject> OnTriggerStayAction;
        public Action<GameObject> OnTriggerExitAction;

        void OnValidate()
        {
            if (!_collider) _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            
            if (IsDesiredLayer(other.gameObject))
            {
                // Debug.Log($"Triggered by {other.name}");
                _onTriggerEnter?.Invoke(other.gameObject);
                OnTriggerEnterAction?.Invoke(other.gameObject);
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (IsDesiredLayer(other.gameObject))
            {
                // Debug.Log($"Triggered by {other.name}");
                _onTriggerStay?.Invoke(other.gameObject);
                OnTriggerStayAction?.Invoke(other.gameObject);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (IsDesiredLayer(other.gameObject))
            {
                // Debug.Log($"Triggered by {other.name}");
                _onTriggerExit?.Invoke(other.gameObject);
                OnTriggerExitAction?.Invoke(other.gameObject);
            }
        }

        public void AddListener(TriggerEventType eventType, UnityAction<GameObject> action)
        {
            switch (eventType)
            {
                case TriggerEventType.Enter:
                    _onTriggerEnter.AddListener(action);
                    break;
                case TriggerEventType.Stay:
                    _onTriggerStay.AddListener(action);
                    break;
                case TriggerEventType.Exit:
                    _onTriggerExit.AddListener(action);
                    break;
            }
        }

        bool IsDesiredLayer(GameObject other)
        {
            return _desiredLayer == (_desiredLayer | (1 << other.layer));
        }
    }
}
