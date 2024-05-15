using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "ComboContainer")]
public class ComboContainer : SkillObject
{
    [SerializeField] private SkillContainer[] comboSkills;
    [SerializeField] private float comboEndTime;

    private int comboIndex;
    private float timer;

    public override SkillContainer CurrentContainer => comboSkills[comboIndex];

    public SkillContainer[] SkillContainers => comboSkills;

    public override void Init()
    {
        comboIndex = 0;
    }

    public override IEnumerator PlaySkill(Transform tr)
    {
        timer = float.MinValue;
        yield return new WaitForSeconds(comboSkills[comboIndex].CoolTime);

        comboIndex = (comboIndex + 1) % comboSkills.Length;

        timer = 0f;
        while (0f <= timer && timer <= comboEndTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (timer > 0f)
        {
            comboIndex = 0;
        }
    }
}