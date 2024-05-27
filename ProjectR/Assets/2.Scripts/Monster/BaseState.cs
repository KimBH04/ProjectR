public abstract class BaseState
{
    protected Monster _monster;
    
    protected BaseState(Monster monster)
    {
        _monster = monster;
    }

    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
}

public class IdleState : BaseState
{
    public IdleState(Monster monster) : base(monster) { }

    public override void OnStateEnter()
    {
        
    }

    public override void OnStateUpdate()
    {
        
    }

    public override void OnStateExit()
    {
        
    }
}

public class MoveState : BaseState
{
    public MoveState(Monster monster) : base(monster) { }

    public override void OnStateEnter()
    {
        // 이동 애니메이션 재생
    }

    public override void OnStateUpdate()
    {
        
    }

    public override void OnStateExit()
    {
        // 이동 애니메이션 종료
    }
}

public class AttackState : BaseState
{
    public AttackState(Monster monster) : base(monster) { }

    public override void OnStateEnter()
    {
        // 공격 애니메이션 재생
    }

    public override void OnStateUpdate()
    {
        
    }

    public override void OnStateExit()
    {
        // 공격 애니메이션 종료
    }
}

public class DieState : BaseState
{
    public DieState(Monster monster) : base(monster) { }

    public override void OnStateEnter()
    {
        // 죽는 애니메이션 재생
    }

    public override void OnStateUpdate()
    {
        
    }

    public override void OnStateExit()
    {
        // 죽는 애니메이션 종료  
    }
}

public class FSM
{
    public FSM(BaseState initState)
    {
        _curState = initState;
        ChangeState(_curState);
    }

    private BaseState _curState;

    public void ChangeState(BaseState nextState)
    {
        if(nextState == _curState)
            return;

        if (_curState != null)
            _curState.OnStateExit();

        _curState = nextState;
        _curState.OnStateEnter();
    }

    public void UpdateState()
    {
        if (_curState != null)
            _curState.OnStateUpdate();
    }
}
