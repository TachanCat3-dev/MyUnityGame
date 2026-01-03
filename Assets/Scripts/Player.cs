using UnityEngine;

public class Player : Entity
{
    // === 玩家特有的状态 ===
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }

    // 
    protected override void Awake()
    {
        base.Awake(); // 关键！这会初始化 rb, anim 和 StateMachine

        // 初始化具体的玩家状态
        // 这里的 'this' 就是 Entity (因为 Player 继承自 Entity)
        IdleState = new PlayerIdleState(this, StateMachine);
        MoveState = new PlayerMoveState(this, StateMachine);
    }

    protected override void Start()
    {
        base.Start();
        // 启动状态机，默认进入 Idle
        StateMachine.Initialize(IdleState);
    }

    // Update 不需要写了，父类 Entity 已经帮我们运行了 stateMachine.UpdateActiveState()
}