public class EnemyModel 
{
    public float MaxHp;
    public float CurrentHp;
    public float Speed;
    public float Damage;
    public float TargetRadius;
    public float TargetRange;

    public EnemyModel(float maxHp, float damage, float speed, float targetRadius, float targetRange)
    {
        MaxHp = maxHp;
        CurrentHp = maxHp;
        Speed = speed;
        Damage = damage;
        TargetRadius = targetRadius;
        TargetRange = targetRange;

    }
}
