using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyView
{
    void UpdateHpBar(float currentHp, float maxHp);
    void ShowDamageText(float damage);
}
