// View
// 디스플레이 + 표시형식 로직
// MonoBehaviour
// 값을 넘겨주면 표시형식을 만드는 정도의 로직은 가지고 있다.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 디스플레이
public class EnemyView : MonoBehaviour
{
    public Image hpBar;
    

    public void UpdateHpBar(float currentHp, float maxHp)
    {
        hpBar.fillAmount = currentHp / maxHp;
    }
}
