using System.Collections;
using UnityEngine;

public abstract class SkillObject : ScriptableObject
{
    /// <summary>
    /// 현재 실행중인 스킬
    /// </summary>
    public abstract SkillContainer CurrentContainer { get; }

    public virtual void Init()
    {
        // todo in child
    }

    public abstract IEnumerator PlaySkill(Transform tr);
}