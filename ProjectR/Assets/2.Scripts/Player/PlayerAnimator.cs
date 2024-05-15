using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    private readonly int hashX = Animator.StringToHash("X"),
                         hashY = Animator.StringToHash("Y"),
                         hashAttack = Animator.StringToHash("Attack"),
                         hashIndex = Animator.StringToHash("Motion Index");

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMovementValue(float x, float y)
    {
        animator.SetFloat(hashX, x);
        animator.SetFloat(hashY, y);
    }

    public void PlayAttack(int motionIndex)
    {
        animator.SetInteger(hashIndex, motionIndex);
        animator.SetTrigger(hashAttack);
    }
}
