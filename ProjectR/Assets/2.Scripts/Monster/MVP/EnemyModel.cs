// 모델에서는 데이터와 비즈니스 로직을 처리 
// 적의 상태와 관련된 것들을 포함
public class EnemyModel
{
    public float MaxHp { get; set; }
    public float CurrentHp { get; set; }

    public EnemyModel(float maxHp)
    {
        MaxHp = maxHp;
        CurrentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        CurrentHp -= damage;
        if (CurrentHp < 0)
        {
            CurrentHp = 0;
        }
    }
}
