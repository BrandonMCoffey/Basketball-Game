using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketballGoal : MonoBehaviour
{
    [SerializeField] private Transform _netTarget;
    [SerializeField] private AnimationCurve _swishAmountCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.1f, 0.5f), new Keyframe(0.3f, -1f), new Keyframe(0.6f, 0.8f),
                                                                new Keyframe(0.9f, -0.5f), new Keyframe(1.2f, 0.2f), new Keyframe(1.6f, -0.1f), new Keyframe(1.8f, 0.05f), new Keyframe(2f, 0f));
    [SerializeField] private AnimationCurve _yMoveCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));
    [SerializeField] private float _swishRotationAmount = 50;
    [SerializeField] private List<Transform> _netSwishBones;

    [SerializeField] private List<ParticleSystem> _flamethrowers = new List<ParticleSystem>();

    public Transform NetTarget => _netTarget;

    [Button, HideInEditorMode]
    public void SwishNet()
    {
        StopAllCoroutines();
        StartCoroutine(SwishNetRoutine());
    }

    private IEnumerator SwishNetRoutine()
    {
        for (float t = 0; t < _swishAmountCurve.length; t += Time.deltaTime)
        {
            float amount = _swishAmountCurve.Evaluate(t) * _swishRotationAmount / _netSwishBones.Count;
            for (int i = 0; i < _netSwishBones.Count; i++)
            {
                _netSwishBones[i].localRotation = Quaternion.Euler(amount * (i + 1), 0f, 0f);
            }
            yield return null;
        }
    }

    public void PlayFlamethrowers()
    {
        foreach (var ps in _flamethrowers)
        {
            ps.Emit(10);
        }
    }
}
