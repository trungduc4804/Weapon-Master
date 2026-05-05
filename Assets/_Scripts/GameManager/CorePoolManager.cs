using System.Collections.Generic;
using UnityEngine;

public class PoolMember : MonoBehaviour
{
    public GameObject prefab;
}

public class CorePoolManager : MonoBehaviour
{
    public static CorePoolManager Instance { get; private set; }

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        if (!pools.ContainsKey(prefab))
        {
            pools[prefab] = new Queue<GameObject>();
        }

        GameObject obj;
        if (pools[prefab].Count > 0)
        {
            obj = pools[prefab].Dequeue();
            if (obj == null)
            {
                return Get(prefab, position, rotation);
            }
            
            // Gán vị trí TRƯỚC khi SetActive để OnEnable lấy đúng tọa độ
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab, position, rotation);
            obj.SetActive(true);
            PoolMember member = obj.AddComponent<PoolMember>();
            member.prefab = prefab;
        }

        return obj;
    }

    public void Release(GameObject obj)
    {
        if (obj == null) return;

        PoolMember member = obj.GetComponent<PoolMember>();
        if (member == null)
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        if (!pools.ContainsKey(member.prefab))
        {
            pools[member.prefab] = new Queue<GameObject>();
        }
        pools[member.prefab].Enqueue(obj);
    }
}
