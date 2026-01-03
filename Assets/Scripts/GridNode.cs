using UnityEngine;

public class GridNode
{
    public int x;
    public int z;
    public Vector3 worldPosition;
    public bool isWalkable;


    // 快捷属性：判断格子上有没有人
    //public bool IsOccupied => occupiedUnit != null;

    // A* 寻路专用参数
    public int gCost;
    public int hCost;
    public GridNode parentNode;

    public int FCost => gCost + hCost;

    public GridNode(int x, int z, Vector3 worldPos, bool isWalkable)
    {
        this.x = x;
        this.z = z;
        this.worldPosition = worldPos;
        this.isWalkable = isWalkable;
    }
}