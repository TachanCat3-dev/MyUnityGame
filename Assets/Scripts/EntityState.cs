using UnityEngine;

public abstract class EntityState
{
    protected Entity entity;
    protected StateMachine stateMachine;

    // 构造函数
    public EntityState(Entity entity, StateMachine stateMachine)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }
}