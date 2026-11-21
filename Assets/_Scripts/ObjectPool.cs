using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // Đảm bảo chỉ có 1 ObjectPool tồn tại và dễ dàng gọi từ các script khác (Singleton Pattern)
    public static ObjectPool Instance { get; private set; }

    // Dictionary để quản lý nhiều kho (Pool) khác nhau, phân biệt bằng tên (tag)
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    [System.Serializable]
    public class Pool
    {
        public string tag;           // Tên gọi của kho (Ví dụ: "EnemyNormal", "CannonBullet")
        public GameObject prefab;    // Mẫu vật để tạo
        public int size;             // Số lượng tạo sẵn từ đầu game
    }

    public List<Pool> pools; // Danh sách các kho bạn muốn tạo ở Inspector

    private void Awake()
    {
        // Khởi tạo Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Quét qua danh sách các kho và tiến hành đúc sẵn đồ
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                // Tạo đối tượng, tắt nó đi và gom vào làm con của ObjectPool cho gọn
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(this.transform);
                objectPool.Enqueue(obj);
            }

            // Thêm kho vừa đúc vào Dictionary
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // Hàm dùng để lấy (Bật) một đối tượng từ trong kho ra
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Không tìm thấy kho (Pool) nào có tên: " + tag);
            return null;
        }

        Queue<GameObject> objectPool = poolDictionary[tag];

        // Nếu hàng đợi rỗng hoặc đối tượng đầu tiên đang còn active (đang sống trên map),
        // tự động khởi tạo thêm prefab mới (auto-expand) để không bị thiếu quái hay giật quái đang đi
        GameObject objectToSpawn = null;
        if (objectPool.Count > 0 && !objectPool.Peek().activeInHierarchy)
        {
            objectToSpawn = objectPool.Dequeue();
        }
        else
        {
            // Tìm prefab tương ứng trong danh sách pools để sinh thêm
            Pool p = pools.Find(x => x.tag == tag);
            if (p != null && p.prefab != null)
            {
                objectToSpawn = Instantiate(p.prefab);
                objectToSpawn.transform.SetParent(this.transform);
            }
            else if (objectPool.Count > 0)
            {
                objectToSpawn = objectPool.Dequeue();
            }
        }

        if (objectToSpawn == null) return null;

        // Cập nhật vị trí, góc xoay và Bật nó lên
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        // Cho đối tượng vào lại cuối hàng đợi để xoay vòng tái sử dụng
        objectPool.Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}