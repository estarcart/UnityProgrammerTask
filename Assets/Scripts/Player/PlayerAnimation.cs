using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerTransform;

    private static readonly int XVelocityParam = Animator.StringToHash("xVelocity");
    private static readonly int YVelocityParam = Animator.StringToHash("yVelocity");
    private static readonly int IsJumpingParam = Animator.StringToHash("isJumping");

    public void UpdateAnimation(Vector2 velocity, bool isGrounded)
    {
        animator.SetFloat(XVelocityParam, Mathf.Abs(velocity.x));
        animator.SetFloat(YVelocityParam, velocity.y);
        animator.SetBool(IsJumpingParam, !isGrounded);

        if (velocity.x > 0.1f)
        {
            playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (velocity.x < -0.1f)
        {
            playerTransform.localScale = new Vector3(-1, 1, 1);
        }
    }
}

