using System.Collections.Generic;
using UnityEngine;

public class SkillEffects : MonoBehaviour
{
    public static SkillEffects Instance { get; private set; }

    [SerializeField] private Transform[] effectsObjects;

    private readonly Dictionary<FX, (Transform, ParticleSystem)> effects = new Dictionary<FX, (Transform, ParticleSystem)>();

    private void Awake()
    {
        Instance = this;

        int effectsCnt = effectsObjects.Length;
        for (int i = 0; i < effectsCnt; i++)
        {
            effects.Add((FX)i, (effectsObjects[i], effectsObjects[i].GetComponentInChildren<ParticleSystem>()));
        }
    }
    
    /// <summary>
    /// 이펙트 플레이
    /// </summary>
    /// <param name="fx"> 이펙트 종류 </param>
    /// <param name="position"> 이펙트 플레이 위치 </param>
    /// <param name="rotation"> 이펙트 플레이 방향 </param>
    public void PlayEffect(FX fx, Vector3 position, Quaternion rotation)
    {
        if (fx == FX.None)
        {
            return;
        }

        (Transform tr, ParticleSystem effect) = effects[fx];
        tr.SetPositionAndRotation(position, rotation);
        effect.Play(true);
    }

    public enum FX
    {
        BasicSmash,
        BasicHit,
        FireBall,
        Flash,
        MeteoImpact,
        None
    }
}
