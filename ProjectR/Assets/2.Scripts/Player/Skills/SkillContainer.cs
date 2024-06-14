using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill Container/Default")]
public class SkillContainer : SkillObject
{
    [Space]
    [SerializeField, Tooltip("스킬 컨테이너에서 FX를 실행시킬지 여부")] protected bool FXPlayInSkill = true;
    [Space]
    [SerializeField] protected SkillEffects.FX fx;
    [Space]
    [SerializeField] private AudioManager.ESfx startEsfx;
    [SerializeField] private AudioManager.ESfx destroyEsfx;
    [Space]
    [SerializeField] private int animationKey;

    [Header("Stat")]
    [SerializeField] private int atk = 10;
    [SerializeField] private float startTime;
    [SerializeField] private float coolTime = 1f;
    [SerializeField] private float needStamina = 1f;

    [Header("Trigger")]
    [SerializeField] private float disableTime;
    [SerializeField] private SkillEffects.FX destroyFx;

    /// <summary>
    /// 공격력
    /// </summary>
    public int ATK => atk;

    /// <summary>
    /// 시작 딜레이
    /// </summary>
    public float StartTime => startTime;

    /// <summary>
    /// 쿨타임
    /// </summary>
    public float CoolTime => coolTime;

    /// <summary>
    /// 필요한 스태미나 / 마나
    /// </summary>
    public float NeedStamina => needStamina;

    /// <summary>
    /// 활성화 시간
    /// </summary>
    public float DisableTime => disableTime;

    /// <summary>
    /// 실행시킬 애니메이션 키
    /// </summary>
    public int AnimationKey => animationKey;

    /// <summary>
    /// 스킬 시작 오디오 클립
    /// </summary>
    public AudioManager.ESfx StartEsfx => startEsfx;

    /// <summary>
    /// 스킬 소멸/파괴 오디오 클립
    /// </summary>
    public AudioManager.ESfx EndEsfx => destroyEsfx;

    /// <summary>
    /// 스킬 이펙트
    /// </summary>
    public SkillEffects.FX FX => fx;

    /// <summary>
    /// 소멸 이펙트
    /// </summary>
    public SkillEffects.FX DestroyFx => destroyFx;

    public override SkillContainer CurrentContainer => this;

    public override IEnumerator PlaySkill()
    {
        yield return new WaitForSeconds(startTime);

        if (FXPlayInSkill)
        {
             SkillEffects.Instance.PlayEffect(fx);
        }

        AudioManager.Instance.PlaySfx(startEsfx);
    }
}