using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class Unit : MonoBehaviour
{
    public float moveSpeed = 5f;
    protected bool isMoving = false;

    // 移动指令
    public async void MoveTo(int targetX, int targetZ, UnityAction onComplete)
    {
        // 1. 获取自己在哪个格子
        GridManager.Instance.GetXZ(transform.position, out int startX, out int startZ);

        // 2. 计算路径
        List<GridNode> path = Pathfinding.FindPath(startX, startZ, targetX, targetZ);

        // 3. 如果有路径，开始移动
        if (path != null && path.Count > 1)
        { // Count > 1 因为包含起点
            isMoving = true;

            // 跳过第一个点（起点），依次走向后续点
            for (int i = 1; i < path.Count; i++)
            {
                Vector3 targetPos = path[i].worldPosition;

                // 简单的平滑移动循环
                while (Vector3.Distance(transform.position, targetPos) > 0.05f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                    // Unity 6 推荐使用 Awaitable 替代 yield return null
                    await Awaitable.NextFrameAsync();
                }
                transform.position = targetPos; // 强制吸附，防止误差
            }

            isMoving = false;
        }
        else
        {
            Debug.Log("无法到达目标或就在原地");
        }

        // 4. 执行回调（比如通知管理器切换回合）
        onComplete?.Invoke();
    }
}