using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private readonly List<Node> touchedNodes = new List<Node>(256);
    private readonly List<Node> neighbourBuffer = new List<Node>(8);
    private readonly List<Node> nodePathBuffer = new List<Node>(128);

    private Heap<Node> openSet;
    private int heapCapacity;

    public bool FindPath(Grid2D grid, Vector3 startWorldPos, Vector3 targetWorldPos, List<Vector3> pathBuffer)
    {
        if (pathBuffer == null)
        {
            throw new ArgumentNullException(nameof(pathBuffer));
        }

        pathBuffer.Clear();

        if (grid == null || grid.MaxSize <= 0)
        {
            return false;
        }

        EnsureHeapCapacity(grid.MaxSize);
        openSet.Clear();
        touchedNodes.Clear();

        Node startNode = grid.NodeFromWorldPoint(startWorldPos);
        Node targetNode = grid.NodeFromWorldPoint(targetWorldPos);

        startNode = grid.FindNearestWalkableNode(startNode, 8);
        targetNode = grid.FindNearestWalkableNode(targetNode, 8);

        if (startNode == null || targetNode == null)
        {
            return false;
        }

        MarkVisited(startNode);
        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode, grid.AllowDiagonal);
        startNode.parent = null;
        startNode.inOpenSet = true;
        openSet.Add(startNode);

        bool foundPath = false;

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            currentNode.inOpenSet = false;
            currentNode.inClosedSet = true;

            if (currentNode == targetNode)
            {
                foundPath = true;
                break;
            }

            grid.GetNeighbours(currentNode, neighbourBuffer);

            for (int i = 0; i < neighbourBuffer.Count; i++)
            {
                Node neighbour = neighbourBuffer[i];

                if (!neighbour.walkable || neighbour.inClosedSet)
                {
                    continue;
                }

                int movementCost = currentNode.gCost + GetDistance(currentNode, neighbour, grid.AllowDiagonal);

                if (!neighbour.inOpenSet || movementCost < neighbour.gCost)
                {
                    MarkVisited(neighbour);
                    neighbour.gCost = movementCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode, grid.AllowDiagonal);
                    neighbour.parent = currentNode;

                    if (!neighbour.inOpenSet)
                    {
                        neighbour.inOpenSet = true;
                        openSet.Add(neighbour);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        if (foundPath)
        {
            RetracePath(startNode, targetNode, pathBuffer);
        }

        ResetTouchedNodes();
        return foundPath;
    }

    private void EnsureHeapCapacity(int requiredCapacity)
    {
        if (openSet != null && heapCapacity >= requiredCapacity)
        {
            return;
        }

        heapCapacity = Mathf.Max(8, requiredCapacity);
        openSet = new Heap<Node>(heapCapacity);
    }

    private void MarkVisited(Node node)
    {
        if (node.visitedInSearch)
        {
            return;
        }

        node.visitedInSearch = true;
        touchedNodes.Add(node);
    }

    private void RetracePath(Node startNode, Node endNode, List<Vector3> pathBuffer)
    {
        nodePathBuffer.Clear();
        Node currentNode = endNode;

        while (currentNode != null && currentNode != startNode)
        {
            nodePathBuffer.Add(currentNode);
            currentNode = currentNode.parent;
        }

        nodePathBuffer.Reverse();

        for (int i = 0; i < nodePathBuffer.Count; i++)
        {
            pathBuffer.Add(nodePathBuffer[i].worldPosition);
        }
    }

    private void ResetTouchedNodes()
    {
        for (int i = 0; i < touchedNodes.Count; i++)
        {
            touchedNodes[i].ResetSearchData();
        }
    }

    private int GetDistance(Node a, Node b, bool allowDiagonal)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        if (allowDiagonal)
        {
            // Octile distance with integer costs: straight=10, diagonal=14.
            if (dstX > dstY)
            {
                return 14 * dstY + 10 * (dstX - dstY);
            }

            return 14 * dstX + 10 * (dstY - dstX);
        }

        return 10 * (dstX + dstY);
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}

public class Heap<T> where T : IHeapItem<T>
{
    private T[] items;
    private int currentItemCount;

    public int Count
    {
        get { return currentItemCount; }
    }

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
        currentItemCount = 0;
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;

        if (currentItemCount > 0)
        {
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
        }

        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public void Clear()
    {
        currentItemCount = 0;
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount
                    && items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                {
                    swapIndex = childIndexRight;
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            if (item.HeapIndex == 0)
            {
                return;
            }

            T parentItem = items[parentIndex];

            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
                parentIndex = (item.HeapIndex - 1) / 2;
            }
            else
            {
                return;
            }
        }
    }

    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}
