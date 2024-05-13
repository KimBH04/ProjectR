using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    private readonly int hashX = Animator.StringToHash("X");
    private readonly int hashY = Animator.StringToHash("Y");

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMovementValue(float x, float y)
    {
        animator.SetFloat(hashX, x);
        animator.SetFloat(hashY, y);
    }
}
