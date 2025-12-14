using UnityEngine;
using System.Collections;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private float wallOffset = 0.02f; // 2 cm from wall
    [SerializeField] private int maxTargets = 3;

    public static TargetSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    private int currentTargets = 0;

    private void Start()
    {
        Debug.Log("🎯 TargetSpawner START");
        StartCoroutine(WaitForWallAndSpawn());
    }

    private IEnumerator WaitForWallAndSpawn()
    {
        while (WallAnchorManager.Instance == null || !WallAnchorManager.Instance.IsReady)
            yield return null;

        Debug.Log("🧱 WallAnchorManager ready. Spawning initial targets.");

        for (int i = 0; i < maxTargets; i++)
            SpawnTarget();
    }

    private void SpawnTarget()
    {
        if (targetPrefab == null)
        {
            Debug.LogError("❌ Target Prefab not assigned in Inspector");
            return;
        }

        if (WallAnchorManager.Instance == null || !WallAnchorManager.Instance.IsReady)
        {
            Debug.LogError("❌ WallAnchorManager not ready");
            return;
        }

        Transform wall = WallAnchorManager.Instance.GetRandomWall();
        if (wall == null)
        {
            Debug.LogError("❌ No wall returned from WallAnchorManager");
            return;
        }

        BoxCollider wallCollider = wall.GetComponent<BoxCollider>();
        if (wallCollider == null)
        {
            Debug.LogError($"❌ Wall '{wall.name}' has no BoxCollider (needed for bounds)");
            return;
        }

        Bounds b = wallCollider.bounds;

        // Random point on the wall plane
        float x = Random.Range(-b.extents.x, b.extents.x);
        float y = Random.Range(-b.extents.y, b.extents.y);

        // Decide which side faces the player (camera)
        Vector3 wallNormal = wall.forward;

        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 toPlayer = cam.transform.position - b.center;
            if (toPlayer.sqrMagnitude > 0.0001f)
            {
                // If wallNormal points away from player, flip it
                if (Vector3.Dot(wallNormal, toPlayer.normalized) < 0f)
                    wallNormal = -wallNormal;
            }
        }

        Vector3 spawnPos =
            b.center +
            wall.right * x +
            wall.up * y +
            wallNormal * wallOffset;

        // Face outward on the player-facing side + fix cylinder axis (X 90°)
        Quaternion rotation =
            Quaternion.LookRotation(wallNormal) *
            Quaternion.Euler(90f, 0f, 0f);

        GameObject target = Instantiate(targetPrefab, spawnPos, rotation);
        target.transform.SetParent(wall, true);

        currentTargets++;
        Debug.Log($"🎯 Target spawned on '{wall.name}' at {spawnPos}");
    }

    public void ResetTargets()
    {
        Debug.Log("🎯 ResetTargets called");

        // Destroy existing targets
        Target[] targets = FindObjectsOfType<Target>();
        foreach (var t in targets)
            Destroy(t.gameObject);

        // Reset count and respawn
        StartCoroutine(RespawnAfterDelay());
    }

    public void NotifyTargetDestroyed()
    {
        currentTargets = Mathf.Max(0, currentTargets - 1);
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        if (currentTargets < maxTargets)
            SpawnTarget();
    }
}
