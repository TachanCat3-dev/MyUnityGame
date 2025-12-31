using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; } // 单例，方便全局访问

    [Header("地图设置")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 2f; // 每个格子的大小
    public LayerMask obstacleLayer; // 障碍物层级

    private GridNode[,] gridArray;

    void Awake()
    {
        Instance = this;
        GenerateGrid();
    }

    void GenerateGrid()
    {
        gridArray = new GridNode[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 worldPoint = GetWorldPosition(x, z);
                // 检测该位置是否有障碍物 (使用 Physics.CheckSphere)
                bool isWalkable = !Physics.CheckSphere(worldPoint, cellSize * 0.4f, obstacleLayer);
                gridArray[x, z] = new GridNode(x, z, worldPoint, isWalkable);
            }
        }
    }

    // 辅助：从网格坐标转世界坐标
    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x * cellSize, 0, z * cellSize) + transform.position;
    }

    // 辅助：从世界坐标转网格坐标
    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition.x - transform.position.x + cellSize * 0.5f) / cellSize);
        z = Mathf.FloorToInt((worldPosition.z - transform.position.z + cellSize * 0.5f) / cellSize);
    }

    // 获取特定格子
    public GridNode GetNode(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            return gridArray[x, z];
        }
        return null;
    }

    // 获取宽度和高度
    public int GetWidth() => width;
    public int GetHeight() => height;

    // 可视化调试（在Scene窗口画格子）
    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 center = new Vector3(x * cellSize, 0, z * cellSize) + transform.position;
                Gizmos.DrawWireCube(center, new Vector3(cellSize, 0.1f, cellSize) * 0.9f);
            }
        }
    }
}