using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : EntityState
{
    private int targetX, targetZ;
    private List<GridNode> path;
    private int currentPathIndex;
    private System.Action onMoveComplete;

    // 构造函数
    public PlayerMoveState(Entity entity, StateMachine stateMachine) : base(entity, stateMachine) { }

    // 设置目标
    public void SetTarget(int x, int z, System.Action onComplete = null)
    {
        this.targetX = x;
        this.targetZ = z;
        this.onMoveComplete = onComplete;
    }

    public override void Enter()
    {
        GridManager.Instance.GetXZ(entity.transform.position, out int startX, out int startZ);
        path = Pathfinding.FindPath(startX, startZ, targetX, targetZ);

        if (path != null && path.Count > 1)
        {
            currentPathIndex = 1;
            // 如果有动画： entity.anim.SetBool("IsWalking", true);
        }
        else
        {
            // 没路走，直接结束
            stateMachine.ChangeState(null);
            onMoveComplete?.Invoke();
        }
    }

    public override void Update()
    {
        if (path == null) return;

        if (currentPathIndex < path.Count)
        {
            Vector3 targetPos = path[currentPathIndex].worldPosition;

            // ✅ 直接使用 entity.moveSpeed
            entity.transform.position = Vector3.MoveTowards(entity.transform.position, targetPos, entity.moveSpeed * Time.deltaTime);

            Vector3 dir = (targetPos - entity.transform.position).normalized;
            if (dir != Vector3.zero)
                entity.transform.forward = Vector3.Lerp(entity.transform.forward, dir, 10f * Time.deltaTime);

            if (Vector3.Distance(entity.transform.position, targetPos) < 0.05f)
            {
                currentPathIndex++;
            }
        }
        else
        {
            Arrive();
        }
    }

    void Arrive()
    {
        path = null;
        // 触发回调
        onMoveComplete?.Invoke();
    }

    public override void Exit()
    {
        // entity.anim.SetBool("IsWalking", false);
        path = null;
    }
}