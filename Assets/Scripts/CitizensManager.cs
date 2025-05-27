using System.Collections.Generic;
using UnityEngine;

public class CitizensManager : MonoBehaviour
{
    public List<Vector2> NPCSpawnPoints = new List<Vector2>();
    public bool InitializeNPcSpawnPoints = false;
    public static CitizensManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public List<GameObject> None = new List<GameObject>();
    public List<GameObject> Archers = new List<GameObject>();
    public List<GameObject> Builders = new List<GameObject>();
    private void Update()
    {
        if (InitializeNPcSpawnPoints)
        {
            InitializeNPCSpawnPoints();
            InitializeNPcSpawnPoints = false;
        }
    }

    public void InitializeNPCSpawnPoints()
    {
        NPCSpawnPoints.Clear();
        foreach (var npc in None)
        {
            if (npc != null && npc.activeInHierarchy)
                NPCSpawnPoints.Add(npc.transform.position);
        }
    }
}
