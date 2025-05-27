using UnityEngine;

public class ReflectionManager : MonoBehaviour
{
    public Camera mainCamera;
    public Camera reflectionCamera;
    public Renderer waterRenderer;
    public float offset = 0.1f;

    void Start()
    {
        if (!reflectionCamera)
        {
            GameObject reflectionCam = new GameObject("URPReflectionCamera");
            reflectionCamera = reflectionCam.AddComponent<Camera>();
            reflectionCamera.orthographic = mainCamera.orthographic;
            reflectionCamera.orthographicSize = mainCamera.orthographicSize;
            reflectionCamera.clearFlags = CameraClearFlags.SolidColor;
            reflectionCamera.backgroundColor = Color.clear;
            reflectionCamera.cullingMask = ~0;  // Tüm katmanlarý içerir
        }

        RenderTexture reflectionTexture = new RenderTexture(1024, 1024, 16);
        reflectionCamera.targetTexture = reflectionTexture;
        waterRenderer.material.SetTexture("_ReflectionTex", reflectionTexture);
    }

    void Update()
    {
        Vector3 camPosition = mainCamera.transform.position;
        camPosition.y = -camPosition.y + offset;
        reflectionCamera.transform.position = camPosition;
        reflectionCamera.transform.rotation = Quaternion.Euler(-mainCamera.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, 0);
    }
}
