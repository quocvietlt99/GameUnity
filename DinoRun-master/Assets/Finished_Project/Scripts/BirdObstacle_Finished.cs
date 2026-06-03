using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdObstacle_Finished : MonoBehaviour
{
    [Header("Bird Height")]
    [Tooltip("Các độ cao chim có thể bay. Spawner sẽ chọn ngẫu nhiên 1 giá trị.")]
    public float[] randomYPositions = new float[]
    {
        0.2f,
        0.8f,
        1.3f
    };

    [Header("Collider")]
    public bool forceTriggerCollider = true;
    public Vector2 colliderSize = new Vector2(0.7f, 0.45f);
    public Vector2 colliderOffset = Vector2.zero;

    [Header("Animation")]
    public bool randomAnimationSpeed = true;
    public float minAnimationSpeed = 0.85f;
    public float maxAnimationSpeed = 1.25f;

    private Animator animator;

    private void Awake()
    {
        gameObject.tag = "Obstacle";

        animator = GetComponent<Animator>();

        if (animator != null && randomAnimationSpeed)
        {
            animator.speed = Random.Range(minAnimationSpeed, maxAnimationSpeed);
        }

        FixCollider();
        EnsureObstacleBlock();
    }

    private void FixCollider()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        if (forceTriggerCollider)
        {
            boxCollider.isTrigger = true;
        }

        boxCollider.size = colliderSize;
        boxCollider.offset = colliderOffset;
    }

    private void EnsureObstacleBlock()
    {
        ObstacleBlock_Finished obstacleBlock = GetComponent<ObstacleBlock_Finished>();

        if (obstacleBlock == null)
        {
            gameObject.AddComponent<ObstacleBlock_Finished>();
        }
    }

    public float GetRandomYPosition()
    {
        if (randomYPositions == null || randomYPositions.Length == 0)
        {
            return transform.position.y;
        }

        int randomIndex = Random.Range(0, randomYPositions.Length);
        return randomYPositions[randomIndex];
    }
}