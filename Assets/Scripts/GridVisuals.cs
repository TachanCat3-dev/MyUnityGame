using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridVisuals : MonoBehaviour
{
    public static GridVisuals Instance { get; private set; }

    private Mesh mesh;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        Instance = this;
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Start()
    {
        // 游戏开始时自动生成网格
        GenerateGridMesh();
        HideGrid(); // 默认先隐藏，等轮到玩家再显示
    }

    public void GenerateGridMesh()
    {
        int width = GridManager.Instance.width;
        int height = GridManager.Instance.height;
        float cellSize = GridManager.Instance.cellSize;

        // 计算需要的顶点数量
        int lineCount = (width + 1) + (height + 1);
        Vector3[] vertices = new Vector3[lineCount * 2];
        int[] indices = new int[lineCount * 2];

        int vIndex = 0;

        // === 核心修改：增加一个偏移量 ===
        // 偏移量 = 负的半个格子大小
        // 这样 (0,0) 就会变成格子的中心，而不是左下角顶点
        float offset = -0.5f * cellSize;

        // 1. 生成竖线 (Vertical Lines)
        for (int x = 0; x <= width; x++)
        {
            // 原来是: float xPos = x * cellSize;
            // 现在加上偏移:
            float xPos = (x * cellSize) + offset;

            // 竖线的 Z 轴起点和终点也要加上偏移，保证网格封闭
            float zStart = 0 + offset;
            float zEnd = (height * cellSize) + offset;

            // 起点
            vertices[vIndex] = new Vector3(xPos, 0.05f, zStart);
            indices[vIndex] = vIndex;
            vIndex++;

            // 终点
            vertices[vIndex] = new Vector3(xPos, 0.05f, zEnd);
            indices[vIndex] = vIndex;
            vIndex++;
        }

        // 2. 生成横线 (Horizontal Lines)
        for (int z = 0; z <= height; z++)
        {
            // 原来是: float zPos = z * cellSize;
            float zPos = (z * cellSize) + offset;

            // 横线的 X 轴起点和终点也要加偏移
            float xStart = 0 + offset;
            float xEnd = (width * cellSize) + offset;

            // 起点
            vertices[vIndex] = new Vector3(xStart, 0.05f, zPos);
            indices[vIndex] = vIndex;
            vIndex++;

            // 终点
            vertices[vIndex] = new Vector3(xEnd, 0.05f, zPos);
            indices[vIndex] = vIndex;
            vIndex++;
        }

        // 3. 应用到 Mesh
        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        mesh.RecalculateBounds();
    }

    public void ShowGrid()
    {
        meshRenderer.enabled = true;
    }

    public void HideGrid()
    {
        meshRenderer.enabled = false;
    }
}