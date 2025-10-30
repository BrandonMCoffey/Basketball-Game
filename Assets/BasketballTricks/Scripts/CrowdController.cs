using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _crowd;
    [SerializeField] private float _crowdPercent = 0.6f;
    [SerializeField] private float _transitionSpeed = 5f;

    [Header("Idle Animation")]
    [SerializeField] private Vector3 _rotationAmount = new Vector3(0f, 15f, 5f);
    [SerializeField] private float _positionAmountY = 0.1f;
    [SerializeField] private Vector2 _speedRange = new Vector2(0.5f, 1.2f);

    [Header("Hype Animation")]
    [SerializeField] private Vector3 _hypeRotationAmount = new Vector3(15f, 180f, 25f);
    [SerializeField] private float _hypePositionAmountY = 1.2f;
    [SerializeField] private Vector2 _hypeSpeedRange = new Vector2(4f, 15f);
    [SerializeField] private float _maxHype = 20f;
    [SerializeField] private float _hypeExponent = 3f;

    private bool _playing;
    private float _hype;

    private float _currentSpeed;
    private float _currentPositionY;
    private Vector3 _currentRotation;

    private struct CrowdPerson
    {
        public Transform Transform;
        public Vector3 Position;
        public Quaternion Rotation;
        public float PosSpeed;
        public float RotSpeed;
        public float TimeOffset;
    }

    private List<CrowdPerson> _crowdPeople;

    public void SetPlaying(bool playing) => _playing = playing;
    public void SetHype(float hype) => _hype = hype + 1;

    private void Start()
    {
        _crowdPeople = new List<CrowdPerson>(_crowd.Count);
        foreach (GameObject obj in _crowd)
        {
            if (Random.value > _crowdPercent)
            {
                obj.SetActive(false);
                continue;
            }
            _crowdPeople.Add(new CrowdPerson
            {
                Transform = obj.transform,
                Position = obj.transform.localPosition,
                Rotation = obj.transform.localRotation,
                PosSpeed = Random.Range(_speedRange.x, _speedRange.y),
                RotSpeed = Random.Range(_speedRange.x, _speedRange.y),
                TimeOffset = Random.Range(0f, 100f)
            });
        }
    }

    private void Update()
    {
        Debug.Log(_hype);
        if (_playing)
        {
            float normalizedHype = Mathf.Clamp01(_hype / _maxHype);
            float easedHype = Mathf.Pow(normalizedHype, _hypeExponent);

            float minHypeEffect = 0.1f;
            easedHype = Mathf.Max(easedHype, minHypeEffect * normalizedHype);

            float targetSpeed = Mathf.Lerp(_hypeSpeedRange.x, _hypeSpeedRange.y, easedHype);
            float targetPosY = Mathf.Lerp(0, _hypePositionAmountY, easedHype);
            Vector3 targetRot = Vector3.Lerp(Vector3.zero, _hypeRotationAmount, easedHype);
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * _transitionSpeed);
            _currentPositionY = Mathf.Lerp(_currentPositionY, targetPosY, Time.deltaTime * _transitionSpeed);
            _currentRotation = Vector3.Lerp(_currentRotation, targetRot, Time.deltaTime * _transitionSpeed);
        }
        else
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, (_speedRange.x + _speedRange.y) / 2f, Time.deltaTime * _transitionSpeed);
            _currentPositionY = Mathf.Lerp(_currentPositionY, _positionAmountY, Time.deltaTime * _transitionSpeed);
            _currentRotation = Vector3.Lerp(_currentRotation, _rotationAmount, Time.deltaTime * _transitionSpeed);
        }

        foreach (var person in _crowdPeople)
        {
            float speed = _playing ? _currentSpeed : person.PosSpeed;
            float time = Time.time + person.TimeOffset;

            float yPos = Mathf.Sin(time * speed) * _currentPositionY;
            person.Transform.localPosition = person.Position + new Vector3(0, yPos, 0);

            float xRot = (Mathf.PerlinNoise(time * speed * 0.7f, person.TimeOffset) * 2f - 1f) * _currentRotation.x;
            float yRot = (Mathf.PerlinNoise(person.TimeOffset, time * speed * 0.5f) * 2f - 1f) * _currentRotation.y;
            float zRot = (Mathf.PerlinNoise(time * speed * 0.6f, time * speed * 0.4f) * 2f - 1f) * _currentRotation.z;
            person.Transform.localRotation = person.Rotation * Quaternion.Euler(xRot, yRot, zRot);
        }
    }

    [Button]
    private void GetCrowd()
    {
        _crowd = new List<GameObject>();
        foreach (Transform t1 in transform)
        {
            foreach (Transform t2 in t1)
            {
                if (t2.gameObject.name.Contains("Person", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    _crowd.Add(t2.gameObject);
                }
            }
            if (t1.gameObject.name.Contains("Person", System.StringComparison.InvariantCultureIgnoreCase))
            {
                _crowd.Add(t1.gameObject);
            }
        }
    }
}
