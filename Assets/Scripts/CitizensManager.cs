using System.Collections.Generic;
using UnityEngine;

public class CitizensManager : MonoBehaviour
{
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
}
