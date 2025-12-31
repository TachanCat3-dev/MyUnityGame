using UnityEngine;
using System.Collections.Generic;

public enum GameState
{
    PlayerTurn, // 等待玩家输入
    PlayerMoving, // 玩家正在移动
    EnemyTurn,  // 怪物思考和移动
    Win,
    Lose
}

public class TurnBasedSystem : MonoBehaviour
{
    public static TurnBasedSystem Instance;

    public GameState currentState;

    public UnitPlayer player;
    public UnitEnemy enemy; // 简单起见，先放一个怪物

    void Awake()
    {
        Instance = this;
        currentState = GameState.PlayerTurn;
    }

    void Update()
    {
        if (currentState == GameState.PlayerTurn)
        {
            HandlePlayerInput();
        }
    }

    void HandlePlayerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 射线检测点击的格子
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                // 获取点击的目标位置
                GridManager.Instance.GetXZ(hit.point, out int targetX, out int targetZ);

                // 简单的状态切换
                ChangeState(GameState.PlayerMoving);

                // 让玩家移动
                player.MoveTo(targetX, targetZ, () => {
                    // 移动结束的回调
                    ChangeState(GameState.EnemyTurn);
                    StartEnemyTurn();
                });
            }
        }
    }

    async void StartEnemyTurn()
    {
        Debug.Log("轮到怪物了...");
        await Awaitable.WaitForSecondsAsync(1f); // 模拟思考时间 (Unity 6 写法)

        // 简单的怪物AI：走向玩家
        GridManager.Instance.GetXZ(player.transform.position, out int pX, out int pZ);

        enemy.MoveTo(pX, pZ, () => {
            Debug.Log("怪物移动完毕");
            ChangeState(GameState.PlayerTurn);
        });
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log("State Changed: " + newState);
    }
}