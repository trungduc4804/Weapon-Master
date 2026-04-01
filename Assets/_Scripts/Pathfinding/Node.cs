using System;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;

    // Search flags are kept on node to avoid HashSet allocations each query.
    internal bool inOpenSet;
    internal bool inClosedSet;
    internal bool visitedInSearch;

    private int heapIndex;

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
        ResetSearchData();
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }

        // Reverse for min-heap behavior in the custom Heap implementation.
        return -compare;
    }

    public void ResetSearchData()
    {
        gCost = 0;
        hCost = 0;
        parent = null;
        inOpenSet = false;
        inClosedSet = false;
        visitedInSearch = false;
        heapIndex = 0;
    }
}
