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
        throw new System.NotImplementedException();
    }

    public override void OnStateUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateExit()
    {
        throw new System.NotImplementedException();
    }
}

public class MoveState : BaseState
{
    public MoveState(Monster monster) : base(monster) { }

    public override void OnStateEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateExit()
    {
        throw new System.NotImplementedException();
    }
}

public class AttackState : BaseState
{
    public AttackState(Monster monster) : base(monster) { }

    public override void OnStateEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateExit()
    {
        throw new System.NotImplementedException();
    }
}

public class DieState : BaseState
{
    public DieState(Monster monster) : base(monster) { }

    public override void OnStateEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateExit()
    {
        throw new System.NotImplementedException();
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
