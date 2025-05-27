using Unity.Cinemachine;
using UnityEngine;

public class Cinemachine : MonoBehaviour
{
    public float[] CinemachineXborder;
    private CinemachineCamera cam;
    private CinemachineFollow follow;
    private characterConflict cr;
    GameObject waterRef;

    private void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        follow = GetComponent<CinemachineFollow>();
        cr = FindFirstObjectByType<characterConflict>();
        waterRef = GameObject.Find("WaterReflection");
    }
    private void Update()
    {
        waterRef.transform.position = new Vector3 (transform.position.x, waterRef.transform.position.y,waterRef.transform.position.z);
        if (cr.transform.position.x <= CinemachineXborder[0])
            follow.enabled = false;
        else if (cr.transform.position.x > CinemachineXborder[0] && cr.transform.position.x < CinemachineXborder[1])
        {
            follow.enabled = true;
        }
        else if (cr.transform.position.x >= CinemachineXborder[1])
            follow.enabled = false;
    }

}
