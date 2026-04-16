using System.Collections.Generic;
using UnityEngine;

public class Grid2D : MonoBehaviour
{
    [Header("Grid Setup")]
    [SerializeField] private float nodeRadius = 0.25f;
    [SerializeField] private bool allowDiagonal = true;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Collider2D roomBoundsCollider;
    [SerializeField] private Vector2 fallbackRoomSize = new Vector2(16f, 16f);
    [SerializeField] private float obstacleCheckScale = 0.95f;

    [Header("Debug")]
    [SerializeField] private bool drawDebugGrid = false;
    [SerializeField] private Color walkableColor = new Color(0.2f, 0.9f, 0.4f, 0.25f);
    [SerializeField] private Color blockedColor = new Color(0.9f, 0.25f, 0.25f, 0.3f);

    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX;
    private int gridSizeY;
    private Bounds gridBounds;

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    public bool AllowDiagonal
    {
        get { return allowDiagonal; }
    }

    public Bounds GridBounds
    {
        get { return gridBounds; }
    }

    private void Awake()
    {
        RebuildGrid();
    }

    public void RebuildGrid()
    {
        nodeDiameter = nodeRadius * 2f;
        ResolveBoundsSource();

        Vector2 roomSize = roomBoundsCollider != null
            ? roomBoundsCollider.bounds.size
            : fallbackRoomSize;

        Vector3 roomCenter = roomBoundsCollider != null
            ? roomBoundsCollider.bounds.center
            : transform.position;

        gridBounds = new Bounds(roomCenter, new Vector3(roomSize.x, roomSize.y, 0.1f));

        gridSizeX = Mathf.Max(1, Mathf.RoundToInt(roomSize.x / nodeDiameter));
        gridSizeY = Mathf.Max(1, Mathf.RoundToInt(roomSize.y / nodeDiameter));
        grid = new Node[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = new Vector3(
            roomCenter.x - roomSize.x * 0.5f,
            roomCenter.y - roomSize.y * 0.5f,
            0f
        );

        float checkRadius = nodeRadius * obstacleCheckScale;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft
                    + Vector3.right * (x * nodeDiameter + nodeRadius)
                    + Vector3.up * (y * nodeDiameter + nodeRadius);

                bool walkable = Physics2D.OverlapCircle(
                    worldPoint,
                    checkRadius,
                    obstacleMask
                ) == null;

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float minX = gridBounds.min.x;
        float minY = gridBounds.min.y;
        float width = Mathf.Max(0.0001f, gridBounds.size.x);
        float height = Mathf.Max(0.0001f, gridBounds.size.y);

        float percentX = Mathf.Clamp01((worldPosition.x - minX) / width);
        float percentY = Mathf.Clamp01((worldPosition.y - minY) / height);

        int x = Mathf.Clamp(Mathf.RoundToInt((gridSizeX - 1) * percentX), 0, gridSizeX - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt((gridSizeY - 1) * percentY), 0, gridSizeY - 1);

        return grid[x, y];
    }

    public void GetNeighbours(Node node, List<Node> neighboursBuffer)
    {
        neighboursBuffer.Clear();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                if (!allowDiagonal && Mathf.Abs(x) + Mathf.Abs(y) == 2)
                {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX < 0 || checkX >= gridSizeX || checkY < 0 || checkY >= gridSizeY)
                {
                    continue;
                }

                // CHỐNG KẸT GÓC (Corner Clipping Fix)
                // Nếu đây là di chuyển chéo, kiểm tra 2 ô liền kề tạo thành góc đó có phải là tường không.
                if (allowDiagonal && Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
                {
                    bool isWalkableHorizontal = grid[node.gridX + x, node.gridY].walkable;
                    bool isWalkableVertical = grid[node.gridX, node.gridY + y].walkable;

                    // Nếu 1 trong 2 ô chắn đường (không walkable), tuyệt đối cấm đi chéo qua đó để tránh va chạm vật lý.
                    if (!isWalkableHorizontal || !isWalkableVertical)
                    {
                        continue;
                    }
                }

                neighboursBuffer.Add(grid[checkX, checkY]);
            }
        }
    }

    public Node FindNearestWalkableNode(Node centerNode, int maxSearchRadius = 6)
    {
        if (centerNode == null)
        {
            return null;
        }

        if (centerNode.walkable)
        {
            return centerNode;
        }

        int bestDistanceSqr = int.MaxValue;
        Node bestNode = null;

        for (int radius = 1; radius <= maxSearchRadius; radius++)
        {
            int minX = Mathf.Max(0, centerNode.gridX - radius);
            int maxX = Mathf.Min(gridSizeX - 1, centerNode.gridX + radius);
            int minY = Mathf.Max(0, centerNode.gridY - radius);
            int maxY = Mathf.Min(gridSizeY - 1, centerNode.gridY + radius);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    bool isBorder = x == minX || x == maxX || y == minY || y == maxY;
                    if (!isBorder)
                    {
                        continue;
                    }

                    Node candidate = grid[x, y];
                    if (!candidate.walkable)
                    {
                        continue;
                    }

                    int dx = x - centerNode.gridX;
                    int dy = y - centerNode.gridY;
                    int distanceSqr = dx * dx + dy * dy;

                    if (distanceSqr < bestDistanceSqr)
                    {
                        bestDistanceSqr = distanceSqr;
                        bestNode = candidate;
                    }
                }
            }

            if (bestNode != null)
            {
                return bestNode;
            }
        }

        return null;
    }

    public bool IsInsideGrid(Vector3 worldPosition)
    {
        return gridBounds.Contains(new Vector3(worldPosition.x, worldPosition.y, gridBounds.center.z));
    }

    private void ResolveBoundsSource()
    {
        if (roomBoundsCollider != null)
        {
            return;
        }

        Room room = GetComponentInParent<Room>();
        if (room != null)
        {
            roomBoundsCollider = room.GetComponent<Collider2D>();
            if (roomBoundsCollider != null)
            {
                return;
            }
        }

        roomBoundsCollider = GetComponentInParent<CompositeCollider2D>();
        if (roomBoundsCollider != null)
        {
            return;
        }

        roomBoundsCollider = GetComponentInParent<BoxCollider2D>();
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDebugGrid)
        {
            return;
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(gridBounds.center, gridBounds.size);

        if (grid == null)
        {
            return;
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node n = grid[x, y];
                Gizmos.color = n.walkable ? walkableColor : blockedColor;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.02f));
            }
        }
    }
}
