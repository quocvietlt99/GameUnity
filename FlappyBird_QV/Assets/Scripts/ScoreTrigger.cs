using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    private bool scored = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (scored) return;

        if (collision.CompareTag("Player"))
        {
            scored = true;

            Score score = FindObjectOfType<Score>();

            if (score != null)
            {
                score.AddScore();
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPoint();
            }
        }
    }
}