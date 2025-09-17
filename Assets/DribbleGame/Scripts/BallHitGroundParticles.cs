using UnityEngine;

public class GroundTouchEffect : MonoBehaviour
{
    public DribbleController dribbleController;
    private ParticleSystem particleSystem;

    private void Awake()
    {
        // Get the ParticleSystem component attached to this GameObject
        particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null)
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
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }
}
