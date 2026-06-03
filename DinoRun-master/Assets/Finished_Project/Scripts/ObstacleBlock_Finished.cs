using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBlock_Finished : MonoBehaviour
{
    [Header("Collision Settings")]
    public string playerTag = "Player";

    private bool hasHitPlayer = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        CheckHitPlayer(other);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckHitPlayer(collision.collider);
    }

    private void CheckHitPlayer(Collider2D other)
    {
        if (hasHitPlayer)
        {
            return;
        }

        if (other == null)
        {
            return;
        }

        if (!other.CompareTag(playerTag))
        {
            return;
        }

        hasHitPlayer = true;

        if (GameManagerScript_Finished.instance != null)
        {
            GameManagerScript_Finished.instance.GameOver();
        }
        else
        {
            Debug.LogWarning("ObstacleBlock_Finished: Không tìm thấy GameManagerScript_Finished.instance.");
        }
    }
}