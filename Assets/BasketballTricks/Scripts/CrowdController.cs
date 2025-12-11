using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CrowdController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _crowd;
    [SerializeField] private Gradient _skinColorGradient;
    [SerializeField] private Gradient _shirtColorGradient;
    [SerializeField, Range(0f, 1f)] private float _crowdPercent = 0.6f;
    [SerializeField] private float _playingTransitionSpeed = 4f;
    [SerializeField] private float _defaultTransitionSpeed = 1f;

    [Header("Idle Animation")]
    [SerializeField] private Vector3 _rotationAmount = new Vector3(0f, 15f, 5f);
    [SerializeField] private float _positionAmountY = 0.1f;
    [SerializeField] private Vector2 _speedRange = new Vector2(0.5f, 1.2f);

    [Header("Hype Animation")]
    [SerializeField] private Vector3 _hypeRotationAmount = new Vector3(15f, 180f, 25f);
    [SerializeField] private float _hypePositionAmountY = 1.2f;
    [SerializeField] private Vector2 _hypeSpeedRange = new Vector2(4f, 15f);
    [SerializeField] private float _minHype = 1f;
    [SerializeField] private float _limitHype = 100f;
    [SerializeField] private float _maxHype = 200f;
    [SerializeField] private float _maxHypeGain = 20f;
    [SerializeField, Range(0, 1)] private float _hypeVsGainBlend = 0.3f;
    [SerializeField] private float _hypeExponent = 1.5f;
    [SerializeField] private float _flashUpdateTime = 0.1f;
    [SerializeField] private float _flashSmooth = 4f;

    private bool _playing;
    private float _hype;
    private float _hypeGain;
    private float _flashTimer;
    private float _timeSinceAction;

    private float _flashAmount;
    private float _currentSpeed;
    private float _currentPositionY;
    private Vector3 _currentRotation;

    private struct CrowdPerson
    {
        public Transform Transform;
        public SpriteRenderer ShirtRenderer;
        public GameObject CameraFlash;
        public Vector3 Position;
        public Quaternion Rotation;
        public float PosSpeed;
        public float RotSpeed;
        public float TimeOffset;
    }

    private List<CrowdPerson> _crowdPeople;

    public void SetPlaying(bool playing)
    {
        _playing = playing;
        _hype = 0;
    }
    public void SetHype(float hype)
    {
        _hypeGain = hype - _hype;
        _hype = hype;
        _timeSinceAction = 0;
    }

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
            var head = obj.transform.GetChild(0).GetComponent<SpriteRenderer>();
            var shirt = obj.transform.GetChild(1).GetComponent<SpriteRenderer>();
            var cameraFlash = obj.transform.GetChild(2).gameObject;
            cameraFlash.SetActive(false);
            head.color = _skinColorGradient.Evaluate(Random.value);
            shirt.color = _shirtColorGradient.Evaluate(Random.value);
            _crowdPeople.Add(new CrowdPerson
            {
                Transform = obj.transform,
                ShirtRenderer = shirt,
                CameraFlash = cameraFlash,
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
        if (_playing)
        {
            _timeSinceAction += Time.deltaTime;
            float normalizedTotalHype = Mathf.Clamp01(Mathf.Clamp(_minHype + _hype, _minHype, _limitHype) / _maxHype);
            float normalizedHypeGain = Mathf.Clamp01(_hypeGain / _maxHypeGain);
            float value = Mathf.Lerp(normalizedTotalHype, normalizedHypeGain, _hypeVsGainBlend);

            float abs = Mathf.Abs(_timeSinceAction - 1f);
            float wave = 0.1f * Mathf.Clamp01(1f - 0.5f * abs * abs);

            float easedHype = Mathf.Pow(value + wave, _hypeExponent);

            _flashAmount = Mathf.Lerp(_flashAmount, value, Time.deltaTime * _flashSmooth);

            float targetSpeed = Mathf.Lerp(_hypeSpeedRange.x, _hypeSpeedRange.y, easedHype);
            float targetPosY = Mathf.Lerp(_positionAmountY, _hypePositionAmountY, easedHype);
            Vector3 targetRot = Vector3.Lerp(Vector3.zero, _hypeRotationAmount, easedHype);
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * _playingTransitionSpeed);
            _currentPositionY = Mathf.Lerp(_currentPositionY, targetPosY, Time.deltaTime * _playingTransitionSpeed);
            _currentRotation = Vector3.Lerp(_currentRotation, targetRot, Time.deltaTime * _playingTransitionSpeed);
        }
        else
        {
            _flashAmount = Mathf.Lerp(_flashAmount, 0, Time.deltaTime * _flashSmooth);
            _currentSpeed = Mathf.Lerp(_currentSpeed, (_speedRange.x + _speedRange.y), Time.deltaTime * _defaultTransitionSpeed);
            _currentPositionY = Mathf.Lerp(_currentPositionY, _positionAmountY, Time.deltaTime * _defaultTransitionSpeed);
            _currentRotation = Vector3.Lerp(_currentRotation, _rotationAmount, Time.deltaTime * _defaultTransitionSpeed);
        }

        bool updateFlash = false;
        _flashTimer -= Time.deltaTime;
        if (_flashTimer <= 0)
        {
            _flashTimer = _flashUpdateTime;
            updateFlash = true;
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

            if (updateFlash) person.CameraFlash.SetActive(_flashAmount > 0 ? Random.value > (1f - _flashAmount) : false);
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
