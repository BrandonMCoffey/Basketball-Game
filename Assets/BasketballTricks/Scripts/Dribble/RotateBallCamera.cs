using UnityEngine;

public class InverseZRotation : MonoBehaviour
{
    // Assign the target object in the inspector
    public Transform target;

    void Update()
    {
        if (target != null)
        {
            // Get the local Z rotation of the target in degrees
            float targetLocalZ = target.localEulerAngles.z;

            // Calculate the inverse rotation around Z axis
            float inverseZ = targetLocalZ;

            // Apply the inverse rotation to this object's local Z axis
            Vector3 currentRotation = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, inverseZ);
        }
    }
}
