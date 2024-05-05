using UnityEngine;

[CreateAssetMenu(fileName = "EnemyModel", menuName = "Enemy/EnemyModel", order = 0)] 
public class EnemyData :ScriptableObject
{
    public float maxHp;
    public float currentHp;
}