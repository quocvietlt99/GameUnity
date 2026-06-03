using UnityEngine;

public class WorldMover : MonoBehaviour
{
    [Header("Move")]
    public float speedMultiplier = 1f;

    [Header("Loop")]
    public bool loopWhenOutOfScreen = true;
    public float leftLimitX = -10f;
    public float resetX = 10f;
    public float randomExtraDistance = 3f;

    [Header("Vertical")]
    public bool randomizeY = false;
    public float minY = -2.2f;
    public float maxY = -2.2f;

    private void Update()
    {
        if (SimpleGameManager.Instance == null)
            return;

        if (!SimpleGameManager.Instance.isPlaying)
            return;

        float speed = SimpleGameManager.Instance.worldSpeed * speedMultiplier;
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (loopWhenOutOfScreen && transform.position.x <= leftLimitX)
        {
            ResetPosition();
        }
    }

    private void ResetPosition()
    {
        float newX = resetX + Random.Range(0f, randomExtraDistance);
        float newY = transform.position.y;

        if (randomizeY)
        {
            newY = Random.Range(minY, maxY);
        }

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}