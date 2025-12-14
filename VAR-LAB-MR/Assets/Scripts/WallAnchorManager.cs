using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections.Generic;

public class WallAnchorManager : MonoBehaviour
{
    public static WallAnchorManager Instance;

    public bool IsReady { get; private set; }

    private List<Transform> wallTransforms = new List<Transform>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Try simulated walls first (Editor / Simulator)
        TryFindSimulatedWalls();
    }

    private void OnEnable()
    {
        if (MRUK.Instance != null)
        {
            MRUK.Instance.RoomCreatedEvent.AddListener(OnRoomCreated);
        }
    }

    private void OnDisable()
    {
        if (MRUK.Instance != null)
        {
            MRUK.Instance.RoomCreatedEvent.RemoveListener(OnRoomCreated);
        }
    }

    // ===== REAL MR WALLS =====
    private void OnRoomCreated(MRUKRoom room)
    {
        wallTransforms.Clear();

        foreach (var anchor in room.Anchors)
        {
            if (anchor.HasLabel("WALL"))
            {
                wallTransforms.Add(anchor.transform);
                Debug.Log("🧱 MR wall detected: " + anchor.name);
            }
        }

        if (wallTransforms.Count > 0)
        {
            IsReady = true;
            Debug.Log("🧱 Using MR walls: " + wallTransforms.Count);
        }
    }

    // ===== SIMULATOR FALLBACK =====
    private void TryFindSimulatedWalls()
    {
        GameObject[] simulatedWalls =
            GameObject.FindGameObjectsWithTag("SimulatedWall");

        if (simulatedWalls.Length == 0)
            return;

        wallTransforms.Clear();

        foreach (var wall in simulatedWalls)
        {
            wallTransforms.Add(wall.transform);
            Debug.Log("🧱 Using simulated wall: " + wall.name);
        }

        IsReady = true;
    }

    public Transform GetRandomWall()
    {
        if (!IsReady || wallTransforms.Count == 0)
            return null;

        return wallTransforms[Random.Range(0, wallTransforms.Count)];
    }
}
