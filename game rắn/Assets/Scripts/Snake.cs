using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;   // Prefab cho mỗi đốt thân của rắn
    public Vector2Int direction = Vector2Int.right;// Hướng di chuyển hiện tại của rắn
    public float speed = 20f;// Tốc độ di chuyển của rắn
    public float speedMultiplier = 1f; // Hệ số tăng tốc độ (dùng khi muốn tăng tốc dần)
    public int initialSize = 4;  // Độ dài ban đầu của rắn
    public bool moveThroughWalls = false; // Cho phép rắn xuyên tường hay không

    private readonly List<Transform> segments = new List<Transform>();
    // Danh sách các đốt thân rắn (phần tử đầu tiên là đầu)
    private Vector2Int input; // Hướng người chơi nhập từ bàn phím
    private float nextUpdate; // Thời điểm cho phép cập nhật di chuyển tiếp theo

    private void Start()  // Hàm Start – được gọi khi bắt đầu game
    {
        ResetState();  // Khởi tạo trạng thái ban đầu của rắn
    }

    private void Update()   // Hàm Update – xử lý input từ người chơi
    {
        // Nếu rắn đang di chuyển theo trục X
        // → chỉ cho phép rẽ lên hoặc xuống
        if (direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                input = Vector2Int.up;
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                input = Vector2Int.down;
            }
        }
        // Nếu rắn đang di chuyển theo trục Y
        // → chỉ cho phép rẽ trái hoặc phải
        else if (direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                input = Vector2Int.right;
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                input = Vector2Int.left;
            }
        }
    }

    private void FixedUpdate()  // Hàm FixedUpdate – xử lý di chuyển theo thời gian cố định
    {
        // Nếu chưa tới thời điểm cập nhật tiếp theo thì không di chuyển
        if (Time.time < nextUpdate) {
            return;
        }


        // Cập nhật hướng di chuyển theo input của người chơi
        if (input != Vector2Int.zero) {
            direction = input;
        }


        // Di chuyển các đốt thân
        // Phải duyệt từ cuối về đầu để tránh các đốt chồng lên nhau
        for (int i = segments.Count - 1; i > 0; i--) {
            segments[i].position = segments[i - 1].position;
        }
        // Di chuyển đầu rắn theo hướng hiện tại
        // Làm tròn để đảm bảo di chuyển đúng ô lưới (grid)
        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);


        // Tính thời điểm cập nhật tiếp theo dựa trên tốc độ
        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
    }

    public void Grow() // Hàm tăng độ dài rắn khi ăn thức ăn
    {
        Transform segment = Instantiate(segmentPrefab);
        // Tạo một đốt thân mới từ prefab
        segment.position = segments[segments.Count - 1].position;    // Đặt đốt mới tại vị trí đốt cuối cùng
        segments.Add(segment);
        // Thêm đốt mới vào danh sách
    }

    public void ResetState()
    // Hàm đặt lại trạng thái ban đầu của rắn
    {
        direction = Vector2Int.right; // Đặt lại hướng và vị trí ban đầu
        transform.position = Vector3.zero;

        // Xóa các đốt thân cũ (bỏ qua đầu rắn)
        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }


        // Làm mới danh sách và thêm lại đầu rắn
        segments.Clear();
        segments.Add(transform);

        // Tạo các đốt thân ban đầu
        for (int i = 0; i < initialSize - 1; i++) {
            Grow();
        }
    }

    public bool Occupies(int x, int y)// Kiểm tra rắn có đang chiếm ô (x, y) hay không
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y) {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other) // Xử lý va chạm với các đối tượng khác
    {
        if (other.gameObject.CompareTag("Food")) // Va chạm với thức ăn
        {
            Grow();
            GameManager.instance.AddScore();
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            GameManager.instance.GameOver();
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            GameManager.instance.GameOver();
        }

    }


    private void Traverse(Transform wall) // Hàm xử lý xuyên qua tường
    {
        Vector3 position = transform.position;
        // Nếu đi ngang → đổi trục X
        if (direction.x != 0f) {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        } else if (direction.y != 0f) {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);// Nếu đi dọc → đổi trục Y
        }

        transform.position = position;
    }

}
