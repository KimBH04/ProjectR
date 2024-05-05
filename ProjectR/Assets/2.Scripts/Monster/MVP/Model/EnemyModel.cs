public class EnemyModel 
{
    public float MaxHp;
    public float CurrentHp;
    public float Damage;
    public float TargetRadius;
    public float TargetRange;

    public EnemyModel(float maxHp, float damage, float targetRadius, float targetRange)
    {
        MaxHp = maxHp;
        CurrentHp = maxHp;
        Damage = damage;
        TargetRadius = targetRadius;
        TargetRange = targetRange;
        
    }
}
