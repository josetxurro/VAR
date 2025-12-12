using UnityEngine;
using Meta;

public class RacketVR : MonoBehaviour
{
    public static RacketVR Instance;

    public AudioSource impactAudioSource;
    public AudioClip bodyImpactClip;
    public AudioClip netImpactClip;
    public float bodyForceMultiplier = 0.02f;
    public float netForceMultiplier = 0.1f;

    public float audioMinInterval = 0.1f;
    private float lastAudioTime = -Mathf.Infinity;

    private Rigidbody rb;
    private Vector3 lastPosition;
    private Vector3 velocity;
    private float sphereRadius = 0.115f;
    private float collisionError = 0.001f;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
        lastPosition = rb.position;
    }

    void FixedUpdate()
    {
        Vector3 currentPosition = rb.position;
        velocity = (currentPosition - lastPosition) / Time.fixedDeltaTime;

        Vector3 move = currentPosition - lastPosition;
        float distance = move.magnitude;

        if (distance > 0f)
        {
            Vector3 remainingMove = move;

            while (remainingMove.magnitude > 0f)
            {
                RaycastHit hit;
                if (Physics.SphereCast(lastPosition, sphereRadius, remainingMove.normalized, out hit, remainingMove.magnitude))
                {
                    Rigidbody ballRb = hit.rigidbody;
                    if (ballRb != null)
                    {
                        bool isNet = hit.collider.CompareTag("Net");
                        float multiplier = isNet ? netForceMultiplier : bodyForceMultiplier;
                        Vector3 appliedVelocity = velocity * multiplier;
                        ballRb.AddForce(appliedVelocity, ForceMode.VelocityChange);
                    }

                    Vector3 contact = remainingMove.normalized * hit.distance;
                    Vector3 penetration = remainingMove - contact;
                    penetration -= hit.normal * Vector3.Dot(penetration, hit.normal);
                    remainingMove = contact + penetration;
                    velocity -= hit.normal * Vector3.Dot(velocity, hit.normal);
                    remainingMove += hit.normal * collisionError;
                    velocity += hit.normal * collisionError;
                    lastPosition += contact;
                }
                else
                {
                    lastPosition += remainingMove;
                    remainingMove = Vector3.zero;
                }
            }
        }
        else
        {
            lastPosition = currentPosition;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitPart = collision.GetContact(0).thisCollider.gameObject;
        bool isNet = hitPart.CompareTag("Net");
        AudioClip clip = isNet ? netImpactClip : bodyImpactClip;

        if (impactAudioSource != null && clip != null)
        {
            float now = Time.time;
            if (impactAudioSource.isPlaying) return;
            if (now - lastAudioTime < audioMinInterval) return;

            float speed = velocity.magnitude;
            float vol = Mathf.Lerp(0.02f, 0.7f, speed / 5f);
            impactAudioSource.PlayOneShot(clip, vol);
            lastAudioTime = now;
        }
    }
}
