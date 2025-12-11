using UnityEngine;

public class GroundTouchEffect : MonoBehaviour
{
    public DribbleController dribbleController;
    private ParticleSystem groundParticleSystem;

    private void Awake()
    {
        // Get the ParticleSystem component attached to this GameObject
        groundParticleSystem = GetComponent<ParticleSystem>();
        if (groundParticleSystem == null)
        {
            Debug.LogWarning("No ParticleSystem found on " + gameObject.name);
        }
    }

    private void OnEnable()
    {
        dribbleController.BallTouchGround += OnBallTouchGround;
    }

    private void OnDisable()
    {
        dribbleController.BallTouchGround -= OnBallTouchGround;
    }

    private void OnBallTouchGround()
    {
        if (groundParticleSystem != null)
        {
            groundParticleSystem.Play();
        }
    }
}
