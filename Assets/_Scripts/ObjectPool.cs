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

        // Lấy đối tượng đầu tiên ra khỏi hàng đợi
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // Cập nhật vị trí, góc xoay và Bật nó lên
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Cho đối tượng vào lại cuối hàng đợi để xoay vòng tái sử dụng
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}