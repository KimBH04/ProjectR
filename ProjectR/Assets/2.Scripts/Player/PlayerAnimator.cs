using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    private readonly int
        hashX = Animator.StringToHash("X"),
        hashY = Animator.StringToHash("Y"),
        hashAttack = Animator.StringToHash("Attack"),
        hashIndex = Animator.StringToHash("Motion Index"),
        hashDodge = Animator.StringToHash("Dodge"),
        hashDie = Animator.StringToHash("Die");

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

    public void AttackEvent(SkillEffects.FX fx)
    {
        SkillEffects.Instance.PlayEffect(fx, transform.position, transform.rotation);
    }

    public void PlayDodge()
    {
        animator.SetTrigger(hashDodge);
    }
}
