using UnityEngine;
using System.Collections;

public class BallSpawner : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public int maxBalls = 20;
    public float forceMagnitude = 10f;

    private int ballCount = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (ballCount < maxBalls)
        {
            SpawnBall();
            ballCount++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnBall()
    {
        GameObject ball = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(ball.transform.up * forceMagnitude, ForceMode.Impulse);
            audioSource.PlayOneShot(audioClip);
        }
    }
}