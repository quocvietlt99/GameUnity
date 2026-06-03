using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdObstacle_Finished : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Destroy")]
    public bool destroyWhenOutOfScreen = true;
    public float destroyOffsetFromCameraLeft = 2f;

    [Header("Render")]
    public string sortingLayerName = "Default";
    public int sortingOrder = 50;

    private void Start()
    {
        gameObject.tag = "Obstacle";

        ForceRenderInFront();
        EnsureColliderAndObstacleBlock();
    }

    private void Update()
    {
        if (GameManagerScript_Finished.instance == null)
        {
            return;
        }

        if (!GameManagerScript_Finished.instance.gameRunning)
        {
            return;
        }

        float finalSpeed = speed;

        if (GameManagerScript_Finished.instance != null)
        {
            finalSpeed = speed * GameManagerScript_Finished.instance.GetSpeedMultiplier();
        }

        transform.Translate(Vector3.left * Time.deltaTime * finalSpeed);

        if (destroyWhenOutOfScreen && IsOutOfCameraLeft())
        {
            Destroy(gameObject);
        }
    }

    private bool IsOutOfCameraLeft()
    {
        if (Camera.main == null)
        {
            return transform.position.x <= -12f;
        }

        Vector3 leftEdgeWorldPosition = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));
        return transform.position.x <= leftEdgeWorldPosition.x - destroyOffsetFromCameraLeft;
    }

    private void ForceRenderInFront()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sortingLayerName = sortingLayerName;
            renderers[i].sortingOrder = sortingOrder;
        }
    }

    private void EnsureColliderAndObstacleBlock()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        boxCollider.isTrigger = true;

        ObstacleBlock_Finished obstacleBlock = GetComponent<ObstacleBlock_Finished>();

        if (obstacleBlock == null)
        {
            gameObject.AddComponent<ObstacleBlock_Finished>();
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}