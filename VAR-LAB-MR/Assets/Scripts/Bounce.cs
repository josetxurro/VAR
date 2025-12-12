using UnityEngine;

public class Bounce : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        // Initialize shader parameters
        _meshRenderer.sharedMaterial.SetVector("_ContactPoint", Vector3.zero);
        _meshRenderer.sharedMaterial.SetVector("_ContactDirection", Vector3.zero);
        _meshRenderer.sharedMaterial.SetFloat("_ContactTime", -100);
        _meshRenderer.sharedMaterial.SetFloat("_ContactMagnitude", 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ensure there is at least one valid contact
        if (collision.contactCount < 1)
            return;

        // Get collision info
        ContactPoint contact = collision.GetContact(0);

        Vector3 contactPointWorld = contact.point;
        Vector3 contactDirectionWorld = contact.normal;

        // Send data to the shader
        _meshRenderer.sharedMaterial.SetFloat("_ContactTime", Time.time);
        _meshRenderer.sharedMaterial.SetVector("_ContactPoint", contactPointWorld);
        _meshRenderer.sharedMaterial.SetVector("_ContactDirection", contactDirectionWorld);

        // Bounce strength based on impact velocity
        float magnitude = collision.relativeVelocity.magnitude;
        _meshRenderer.sharedMaterial.SetFloat("_ContactMagnitude", magnitude);
    }

    void OnDestroy()
    {
        // Reset timer so the shader stops bouncing
        if (_meshRenderer != null)
        {
            _meshRenderer.sharedMaterial.SetFloat("_ContactTime", -100);
        }
    }
}