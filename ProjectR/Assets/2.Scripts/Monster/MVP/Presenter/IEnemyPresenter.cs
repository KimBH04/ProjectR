using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyPresenter
{
   IEnumerator AttackPlayer();
   void TakeDamage(float damage);
   IEnumerator OnDamage();

   void DieEnemy();

   IEnumerator OnDie();

}
