using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private Vector3 _rotateAxis = Vector3.up;
    [SerializeField] private float _rotateSpeed = 5;

    void Update()
    {
        transform.Rotate(_rotateAxis, _rotateSpeed * Time.deltaTime);
    }
}
