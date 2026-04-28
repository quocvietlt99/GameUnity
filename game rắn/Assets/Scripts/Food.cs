using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Food : MonoBehaviour
{
    public Collider2D gridArea; // Khu vực lưới (grid) mà thức ăn được sinh ra
    private Snake snake;  // Tham chiếu tới con rắn

    private void Awake()
    // Hàm Awake được gọi trước Start
    {
        snake = FindObjectOfType<Snake>();// Tìm đối tượng snake trong Scene
    }

    private void Start()
    {
        RandomizePosition();// Tạo vị trí ngẫu nhiên ban đầu cho thức ăn
    }

    public void RandomizePosition()// Tạo vị trí ngẫu nhiên ban đầu cho thức ăn
    {
        Bounds bounds = gridArea.bounds; // Lấy giới hạn của khu vực lưới

        // Chọn tọa độ ngẫu nhiên trong giới hạn
        // Làm tròn để khớp với lưới (grid)
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        // Tránh sinh thức ăn trùng với vị trí của rắn
        while (snake.Occupies(x, y))
        {
            x++;

            if (x > bounds.max.x)// Nếu vượt quá giới hạn X thì quay về đầu hàng và tăng Y
            {
                x = Mathf.RoundToInt(bounds.min.x);
                y++;

                if (y > bounds.max.y) // Nếu vượt quá giới hạn Y thì quay về đầu cột
                {
                    y = Mathf.RoundToInt(bounds.min.y);
                }
            }
        }

        transform.position = new Vector2(x, y);    // Gán vị trí mới cho thức ăn
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Snake"))
        {
            RandomizePosition();
        }
    }


}