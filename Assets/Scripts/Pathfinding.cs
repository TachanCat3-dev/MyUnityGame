using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{

    // 核心寻路方法
    public static List<GridNode> FindPath(int startX, int startZ, int endX, int endZ)
    {
        GridNode startNode = GridManager.Instance.GetNode(startX, startZ);
        GridNode endNode = GridManager.Instance.GetNode(endX, endZ);

        if (startNode == null || endNode == null || !endNode.isWalkable) return null;

        List<GridNode> openList = new List<GridNode> { startNode };
        HashSet<GridNode> closedList = new HashSet<GridNode>();

        // 初始化所有节点数据（简单重置一下，实际项目可以优化）
        for (int x = 0; x < GridManager.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < GridManager.Instance.GetHeight(); z++)
            {
                GridNode node = GridManager.Instance.GetNode(x, z);
                node.gCost = int.MaxValue;
                node.parentNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);

        while (openList.Count > 0)
        {
            GridNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                return CalculatePath(endNode); // 找到路径
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (GridNode neighbor in GetNeighbors(currentNode))
            {
                if (closedList.Contains(neighbor)) continue;
                if (!neighbor.isWalkable) { closedList.Add(neighbor); continue; }

                int tentativeGCost = currentNode.gCost + 10; // 假设每格代价为10
                if (tentativeGCost < neighbor.gCost)
                {
                    neighbor.parentNode = currentNode;
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = CalculateDistance(neighbor, endNode);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null; // 没找到路径
    }

    private static List<GridNode> GetNeighbors(GridNode node)
    {
        List<GridNode> neighbors = new List<GridNode>();
        // 正四边形寻路：只检查 上下左右
        int[] xDirs = { 0, 0, -1, 1 };
        int[] zDirs = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            GridNode neighbor = GridManager.Instance.GetNode(node.x + xDirs[i], node.z + zDirs[i]);
            if (neighbor != null) neighbors.Add(neighbor);
        }
        return neighbors;
    }

    private static List<GridNode> CalculatePath(GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        path.Add(endNode);
        GridNode currentNode = endNode;
        while (currentNode.parentNode != null)
        {
            path.Add(currentNode.parentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();
        return path;
    }

    private static int CalculateDistance(GridNode a, GridNode b)
    {
        // 曼哈顿距离（适合网格）
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z);
    }

    private static GridNode GetLowestFCostNode(List<GridNode> pathNodeList)
    {
        GridNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].FCost < lowestFCostNode.FCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}