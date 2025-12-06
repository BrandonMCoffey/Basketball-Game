using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] private bool _onlyRotateY = true;
    [SerializeField] private Vector3 _offsetRotation = Vector3.zero;

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (_onlyRotateY)
        {
            var angles = transform.localEulerAngles;
            transform.LookAt(_camera.transform.position);
            transform.localRotation = Quaternion.Euler(new Vector3(angles.x, transform.localEulerAngles.y + _offsetRotation.y, angles.z));
        }
        else
        {
            transform.LookAt(_camera.transform.position);
            transform.rotation *= Quaternion.Euler(_offsetRotation);
        }
    }
}
