using UnityEngine;

public class VelocityBasedSquashStretch : MonoBehaviour
{
    [Header("Target Object To Track Velocity")]
    [Tooltip("The GameObject whose velocity is used for rotation and squash/stretch.")]
    [SerializeField] private GameObject targetObject;

    [Header("Squash and Stretch Settings")]
    [Tooltip("Multiplier for stretch intensity based on velocity magnitude.")]
    public float velocityStretchMultiplier = 0.1f;

    [Tooltip("Multiplier for squash intensity based on change in velocity direction (degrees).")]
    public float directionChangeSquashMultiplier = 0.01f;

    [Tooltip("Minimum scale along any axis.")]
    public float minScale = 0.5f;

    [Tooltip("Maximum scale along any axis.")]
    public float maxScale = 2f;

    [Tooltip("Smoothness of scale interpolation.")]
    public float scaleLerpSpeed = 5f;

    private Rigidbody targetRigidbody;
    private Vector3 lastVelocity;
    private Vector2 lastVelocityDir = Vector2.up;

    private Vector3 targetScale;

    private void Awake()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("Target Object is not assigned! VelocityBasedSquashStretch will not work.");
            enabled = false;
            return;
        }

        targetRigidbody = targetObject.GetComponent<Rigidbody>();

        if (targetRigidbody == null)
        {
            Debug.LogWarning("Target Object does not have a Rigidbody! VelocityBasedSquashStretch will not work.");
            enabled = false;
            return;
        }

        lastVelocity = targetRigidbody.velocity;
        Vector3 initialVelocity = lastVelocity;
        if (initialVelocity.sqrMagnitude > 0.0001f)
            lastVelocityDir = new Vector2(initialVelocity.x, initialVelocity.y).normalized;
        else
            lastVelocityDir = Vector2.up;

        targetScale = transform.localScale;
    }

    private void Update()
    {
        Vector3 velocity = targetRigidbody.velocity;

        // Project velocity onto XY plane
        Vector2 velocityXY = new Vector2(velocity.x, velocity.y);
        float speed = velocityXY.magnitude;

        if (speed > 0.0001f)
        {
            Vector2 velocityDir = velocityXY.normalized;

            // Calculate angle difference between current and last velocity directions
            float directionChangeAngle = Vector2.SignedAngle(lastVelocityDir, velocityDir);
            float absDirectionChange = Mathf.Abs(directionChangeAngle);

            // Rotate to align local Y with velocity direction (around local Z axis)
            Vector3 localYAxisWorld = transform.up;
            Vector2 localYXY = new Vector2(localYAxisWorld.x, localYAxisWorld.y).normalized;
            float angleToRotate = Vector2.SignedAngle(localYXY, velocityDir);
            transform.Rotate(0f, 0f, angleToRotate, Space.Self);

            // Calculate stretch and squash amounts
            float stretchAmount = Mathf.Clamp(1f + speed * velocityStretchMultiplier, minScale, maxScale);

            // Squash based on directional change - higher angle change = more squash along Y, stretch X
            float squashAmount = Mathf.Clamp(1f - absDirectionChange * directionChangeSquashMultiplier, minScale, maxScale);

            // Combine stretch and squash:
            // Stretch local Y axis by stretchAmount but also squash by squashAmount
            // Squash local X axis opposite of Y (if Y squashed, X stretched)
            float finalScaleY = Mathf.Clamp(stretchAmount * squashAmount, minScale, maxScale);
            float finalScaleX = Mathf.Clamp(1f / finalScaleY, minScale, maxScale);

            // Keep Z scale constant for now
            float finalScaleZ = 1f;

            targetScale = new Vector3(finalScaleX, finalScaleY, finalScaleZ);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleLerpSpeed);

            lastVelocityDir = velocityDir;
        }
        else
        {
            // If velocity very small, reset to default scale smoothly
            targetScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * scaleLerpSpeed);
            transform.localScale = targetScale;
        }

        lastVelocity = velocity;
    }
}
