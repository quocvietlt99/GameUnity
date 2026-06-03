using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject_Finished : MonoBehaviour
{
    public float speed = 5f;

    [Header("Destroy")]
    public bool destroyWhenOutOfScreen = true;
    public float destroyOffsetFromCameraLeft = 2f;

    void Update()
    {
        if (GameManagerScript_Finished.instance == null)
        {
            return;
        }

        if (!GameManagerScript_Finished.instance.gameRunning)
        {
            return;
        }

        float finalSpeed = speed * GameManagerScript_Finished.instance.GetSpeedMultiplier();

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
}