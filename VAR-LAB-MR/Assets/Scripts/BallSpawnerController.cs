using System.Collections;
using UnityEngine;

public class BallSpawnerController : MonoBehaviour
{
    public float amplitude = 0.05f;
    public float frequency = 1.0f;
    public GameObject propeller;

    private Vector3 startPosition;

    public AudioSource audioSource;
    public AudioClip audioClip;

    public GameObject ballPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public int maxBalls = 20;
    public float forceMagnitude = 0.2f;

    public Transform topPart;
    public Transform tubePart;

    public float rotationDuration = 0.8f;

    private Quaternion topCurrentLocal;
    private Quaternion tubeCurrentLocal;

    private int ballCount = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        topCurrentLocal = topPart.localRotation;
        tubeCurrentLocal = tubePart.localRotation;
        StartCoroutine(SpawnRoutine());
    }

    void Start()
    {
        startPosition = transform.localPosition;
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

        StartCoroutine(RotateParts());
    }

    IEnumerator RotateParts()
    {
        float topY = UnityEngine.Random.Range(-45f, 45f);
        float tubeX = UnityEngine.Random.Range(-25f, 4f);

        Quaternion topTarget = Quaternion.Euler(topPart.localEulerAngles.x, topY, topPart.localEulerAngles.z);
        Quaternion tubeTarget = Quaternion.Euler(tubeX, tubePart.localEulerAngles.y, tubePart.localEulerAngles.z);

        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotationDuration;

            topPart.localRotation = Quaternion.Slerp(topCurrentLocal, topTarget, t);
            tubePart.localRotation = Quaternion.Slerp(tubeCurrentLocal, tubeTarget, t);

            yield return null;
        }

        topPart.localRotation = topTarget;
        tubePart.localRotation = tubeTarget;

        topCurrentLocal = topTarget;
        tubeCurrentLocal = tubeTarget;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = startPosition + Vector3.up * offset;
        propeller.transform.Rotate(Vector3.up, 360f * Time.deltaTime);
    }
}
