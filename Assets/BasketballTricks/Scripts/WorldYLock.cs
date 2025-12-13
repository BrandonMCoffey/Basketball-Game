using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways] // Runs in Edit Mode so you can see the effect without hitting Play
public class WorldYLock : MonoBehaviour
{
    [Tooltip("The fixed Y position in World Space you want to lock to.")]
    public float lockedYPosition = 0f;

    [Tooltip("If true, the object will snap to this position immediately in Edit mode.")]
    public bool lockInEditMode = true;

    // LateUpdate is called after all Update functions have been called.
    // This ensures we override any movement applied by parents or animations during the frame.
    void LateUpdate()
    {
        if (!Application.isPlaying && !lockInEditMode) return;

        LockPosition();
    }

    void LockPosition()
    {
        // Get the current world position
        Vector3 currentPosition = transform.position;

        // Create a new vector with the locked Y, but keep current X and Z
        Vector3 newPosition = new Vector3(currentPosition.x, lockedYPosition, currentPosition.z);

        // Apply the new world position
        transform.position = newPosition;
    }
}