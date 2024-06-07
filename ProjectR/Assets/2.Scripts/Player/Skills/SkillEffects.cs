using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillEffects : MonoBehaviour
{
    public static SkillEffects Instance { get; private set; }

    [SerializeField] private Transform[] effectsObjects;

    private readonly Dictionary<FX, (Transform, ParticleSystem)> effects = new Dictionary<FX, (Transform, ParticleSystem)>();
    private static readonly Type FXTYPE = typeof(FX);

    private void Awake()
    {
        Instance = this;

        int effectsCnt = effectsObjects.Length;
        for (int i = 0; i < effectsCnt; i++)
        {
            effects.Add((FX)i, (effectsObjects[i], effectsObjects[i].GetComponentInChildren<ParticleSystem>(true)));
        }
    }

    public (Transform, ParticleSystem) GetParticleObject(FX fx)
    {
        return effects[fx];
    }

    /// <summary>
    /// 이펙트 플레이<br/>
    /// 단순 위치 설정은 <see cref="SetEffectTransform(FX, Vector3, Quaternion)"/>을 사용하십시오
    /// </summary>
    /// <param name="fx"> 이펙트 종류 </param>
    /// <param name="position"> 이펙트 플레이 위치 </param>
    /// <param name="rotation"> 이펙트 플레이 방향 </param>
    public void PlayEffect(FX fx, bool setPosAndRot = true, Vector3 position = default, Quaternion rotation = default)
    {
        if (fx == FX.None || !Enum.IsDefined(FXTYPE, fx) || !effects.ContainsKey(fx))
        {
            return;
        }

        (Transform tr, ParticleSystem effect) = effects[fx];
        if (setPosAndRot)
        {
            tr.SetPositionAndRotation(position, rotation);
        }
        tr.gameObject.SetActive(true);
        effect.Play(true);
    }

    public void SetEffectTransform(FX fx, Vector3 position, Quaternion rotation)
    {
        (Transform tr, _) = effects[fx];
        tr.SetPositionAndRotation(position, rotation);
    }

    public IEnumerator FollowEffect(FX fx, Transform target, float time)
    {
        (Transform tr, _) = effects[fx];

        float currentTime = 0f;
        while (currentTime < time)
        {
            tr.SetPositionAndRotation(target.position, target.rotation);

            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    public enum FX
    {
        Slash1,
        Slash2,
        Slash3,
        Fire, 
        None
    }
}
