using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Enemy/BossData")] 
public class BossData :ScriptableObject
{
    public float maxHp;
    public float currentHp;
    public float speed;
    public float damage;
}