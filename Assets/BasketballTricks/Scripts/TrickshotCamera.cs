using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickshotCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _normalCamera;
    [SerializeField] private CinemachineVirtualCamera _trickCamera;
    [SerializeField] private Transform _trickCameraTarget1;
    [SerializeField] private Transform _trickCameraTarget2;
    [SerializeField] private float _blendSpeed = 5.0f;

    private bool _trickMode = false;
    private Transform _trickFocus1;
    private Transform _trickFocus2;

    private void Update()
    {
        if (_trickMode)
        {
            _trickCameraTarget1.position = Vector3.Lerp(_trickCameraTarget1.position, _trickFocus1.position, _blendSpeed * Time.deltaTime);
            _trickCameraTarget2.position = Vector3.Lerp(_trickCameraTarget2.position, _trickFocus2.position, _blendSpeed * Time.deltaTime);
        }
    }

    public void SetNormalCamera()
    {
        _trickMode = false;
        _normalCamera.Priority = 10;
        _trickCamera.Priority = 0;
    }

    public void SetTrickCamera(Transform startTarget)
    {
        _trickMode = true;
        _normalCamera.Priority = 0;
        _trickCamera.Priority = 10;
        _trickFocus1 = startTarget;
        _trickFocus2 = startTarget;
        _trickCameraTarget1.position = startTarget.transform.position;
        _trickCameraTarget2.position = startTarget.transform.position;
    }

    public void SetTrickFocus(Transform target1, Transform target2 = null)
    {
        _trickFocus1 = target1;
        _trickFocus2 = target2 != null ? target2 : target1;
    }
}
