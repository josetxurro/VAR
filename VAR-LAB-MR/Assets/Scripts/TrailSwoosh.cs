using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TrailSwooshNoOverlap : MonoBehaviour
{
    private AudioSource audioSource;
    private Vector3 lastPosition;

    public AudioClip swooshClip;
    public float minDistance = 0.15f;
    public float maxDistance = 1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lastPosition = transform.position;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (distance >= minDistance && swooshClip != null && !audioSource.isPlaying)
        {
            float t = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
            float volume = Mathf.Lerp(0.1f, 0.4f, t);
            audioSource.PlayOneShot(swooshClip, volume);
        }
    }
}