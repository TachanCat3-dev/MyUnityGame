using UnityEngine;
using System.Collections.Generic;

public enum GameState
{
    PlayerTurn,
    PlayerMoving,
    EnemyTurn,
    Win,
    Lose
}

public class TurnBasedSystem : MonoBehaviour
{
    public static TurnBasedSystem Instance;
    public GameState currentState;

    public Player player;
    public Enemy enemy;

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
            // 只有实例存在才调用，防止报错
            if (GridVisuals.Instance != null) GridVisuals.Instance.ShowGrid();
        }
    }

    void HandlePlayerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                GridManager.Instance.GetXZ(hit.point, out int targetX, out int targetZ);

                // === 修正点：使用 player.MoveState (属性) ===
                player.MoveState.SetTarget(targetX, targetZ, () => 
                {
                    Debug.Log("玩家移动结束");

                    // 切回 Idle
                    player.StateMachine.ChangeState(player.IdleState);

                    // 切到怪物回合
                    ChangeState(GameState.EnemyTurn);
                    StartEnemyTurn();
                });

                // 切换到移动状态
                player.StateMachine.ChangeState(player.MoveState);

                ChangeState(GameState.PlayerMoving);
            }
        }
    }

    async void StartEnemyTurn()
    {
        Debug.Log("轮到怪物了...");
        await Awaitable.WaitForSecondsAsync(1f);

        // 简单的怪物AI... 
        // enemy.MoveTo... (如果 enemy 还没改好，先把这行注释掉)

        Debug.Log("怪物回合结束");
        ChangeState(GameState.PlayerTurn);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        // Debug.Log("State Changed: " + newState);
    }
}