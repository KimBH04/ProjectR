using UnityEngine.UI;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    public Image hpBar;
    
    public void UpdateHpBar(float currentHp, float maxHp)
    {
        hpBar.rectTransform.localScale = new Vector3(currentHp / maxHp, 1, 1);
    }
    
    
}
