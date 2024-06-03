using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData", order = 0)] 
public class EnemyData :ScriptableObject
{
    public float maxHp;
    public float currentHp;
    public float speed;
    public float damage;
    public float targetRadius;
    public float targetRange;
    public float spawnProbability;
    
}