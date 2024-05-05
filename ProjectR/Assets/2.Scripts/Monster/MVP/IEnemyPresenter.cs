using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyPresenter
{
    void Attack();
    void TakeDamage(float damage);
    void Die();
}
