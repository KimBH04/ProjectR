using UnityEngine.UI;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    public Image hpBar;
    public GameObject damageText;
    public Transform damageTextPos;
    
    public void UpdateHpBar(float currentHp, float maxHp)
    {
        hpBar.rectTransform.localScale = new Vector3(currentHp / maxHp, 1, 1);
    }

    public void ShowDamageText(float damage)
    {
        GameObject text = Instantiate(damageText, damageTextPos);
        text.GetComponent<DamageText>().damage = damage;
    }
    
    
}
