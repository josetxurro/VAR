using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private int points = 10;
    private bool hit = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hit) return;

        if (collision.gameObject.CompareTag("Ball"))
        {
            hit = true;

            // Add score
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.AddPoints(points);
            }

            // Notify spawner
            TargetSpawner spawner = FindObjectOfType<TargetSpawner>();
            if (spawner != null)
            {
                spawner.NotifyTargetDestroyed();
            }

            Destroy(gameObject);
        }
    }
}
