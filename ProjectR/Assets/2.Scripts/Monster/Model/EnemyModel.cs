public struct EnemyModel 
{
    public string EnemyName;
    public float MaxHp;
    public float CurrentHp;
    public float Speed;
    public float Damage;
    public float TargetRadius;
    public float TargetRange;

    public EnemyModel(string name,float maxHp, float damage, float speed, float targetRadius, float targetRange)
    {
        EnemyName = name;
        MaxHp = maxHp;
        CurrentHp = maxHp;
        Speed = speed;
        Damage = damage;
        TargetRadius = targetRadius;
        TargetRange = targetRange;

    }
}
