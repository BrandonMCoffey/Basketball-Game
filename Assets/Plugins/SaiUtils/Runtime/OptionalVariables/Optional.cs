using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaiUtils.OptionalVariables
{
    [System.Serializable]
    public struct Optional<T>
    {
        [SerializeField] private bool _hasValue;
        [SerializeField] private T _value;

        public bool HasValue => _hasValue;
        public T Value => _value;

        public Optional(T value)
        {
            _hasValue = true;
            _value = value;
        }
    }
}
