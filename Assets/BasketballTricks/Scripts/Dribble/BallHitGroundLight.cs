using UnityEngine;

public class BallGroundListener : MonoBehaviour
{
    public DribbleController dribbleController; // Reference to the script that fires the event
    public string animationClipName; // Name of the animation clip to play

    private Animation anim;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        if (anim == null)
        {
            Debug.LogError("No Animation component found on this GameObject.");
        }
    }

    private void OnEnable()
    {
        if (dribbleController != null)
        {
            Debug.Log("playing animation");
            dribbleController.BallTouchGround += PlayAnimationOnTouch;
        }
        else
        {
            Debug.LogError("DribbleController reference not set.");
        }
    }

    private void OnDisable()
    {
        if (dribbleController != null)
        {
            dribbleController.BallTouchGround -= PlayAnimationOnTouch;
        }
    }

    private void PlayAnimationOnTouch()
    {
        if (anim != null && animClipExists(animationClipName))
        {
            anim.Play(animationClipName);
        }
        else
        {
            Debug.LogWarning("Animation clip not found or Animation component missing.");
        }
    }

    private bool animClipExists(string clipName)
    {
        foreach (AnimationState state in anim)
        {
            if (state.name == clipName)
                return true;
        }
        return false;
    }
}
