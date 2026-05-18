using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    public float speed = 1.5f;
    public float resetPosition = -10f;
    public float startPosition = 10f;

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x <= resetPosition)
        {
            transform.position = new Vector3(startPosition, transform.position.y, transform.position.z);
        }
    }
}