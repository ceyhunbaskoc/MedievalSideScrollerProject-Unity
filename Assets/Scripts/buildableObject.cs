using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UI;

public class buildableObject : MonoBehaviour
{
    public List<GameObject> BuildingsPrefabs = new List<GameObject>();
    public Transform canvas;
    public int currentCoinCount = 0;
    public float transitionTime;
    protected characterConflict cr;

    [Header("Canvas Properties")]
    public float canvasPositionY; // Inspector’dan ayarlanacak
    public Vector3 canvasScale = new Vector3(1,1,1);     // Inspector’dan ayarlanacak
    public Vector2 canvasSize;

    private GameObject canvasHolder;
    private GameObject canvasCreate;
    private Canvas BuildableCanvas; 
    private RectTransform canvasRect;

    public GameObject slotPrefab;         // Oluşturulacak slotun prefabı (UI Image önerilir)
    public Transform Canvas;              // Slotların ekleneceği World Space Canvas (Transform veya RectTransform olabilir)
    public int totalSlots = 8;            // Oluşturulacak toplam slot sayısı
    public int maxSlotsPerRow = 8;        // Bir satırda maksimum kaç slot olacak
    public float arcAngle = 120f;         // Hilalin toplam açısı (derece cinsinden)
    public float fixedRadius = 1.0f;      // İlk satırın yarıçapı (World Space için 1.0 iyi bir başlangıç)
    public float rowSpacing = 0.3f;       // Satırlar arası mesafe (World Space için)


    // Her satıra en fazla kaç slot yerleştirilsin
    public List<GameObject> coinSlots = new List<GameObject>();
    [Space]
    public List<GameObject> coins = new List<GameObject>();
    public bool isDroppingCoins = false, isConstruction = false,centerDistanceCheck;
    public int whatBuilding, currentBuildersOnThis = 0;
    public int requiredCoin;
    public Sprite isBuildingSpr;
    protected GameObject building;
    float preGrav;
    protected SpriteRenderer sr;


    public float totalBuildTime = 10f; // Tamamlanma süresi (tek inşaatçıyla)
    public float currentProgress = 0f; // 0-100 arası ilerleme
    public float progressPerSecond = 10f; // Her inşaatçı başına saniyede ne kadar ilerlesin

    protected virtual void Start()
    {
        cr = FindObjectOfType<characterConflict>();
        sr = GetComponent<SpriteRenderer>();
        //FindCanvasChilds();
        CreateCanvasHolder();
        GenerateSlots();
    }
    protected virtual void Update()
    {
        if (coins.Count == coinSlots.Count)
        {
            StartCoroutine(WaitForConstruction());
        }
        if (currentBuildersOnThis > 0)
        {
            if (!isConstruction) return;

            float buildAmount = progressPerSecond * currentBuildersOnThis * Time.deltaTime;
            currentProgress += buildAmount;

            currentProgress = Mathf.Clamp(currentProgress, 0f, 100f);

            if (currentProgress >= 100f)
            {
                building = Instantiate(BuildingsPrefabs[whatBuilding], transform.position, Quaternion.identity);
                isConstruction = false;
                cr.onBuildable = false;
                Destroy(gameObject);
            }
        }

    }
    void CreateCanvasHolder()
    {
        // Boş objeyi oluştur
        if (!GameObject.Find("CanvasHolder"))
            canvasHolder = new GameObject("CanvasHolder");
        else
            canvasHolder = GameObject.Find("CanvasHolder");
        canvasCreate = new GameObject(gameObject.name+"Canvas");
        canvasCreate.transform.position = new Vector3(transform.position.x,canvasPositionY,0);
        canvasCreate.transform.localRotation = Quaternion.identity;
        canvasCreate.transform.localScale = canvasScale;
        canvasCreate.transform.SetParent(canvasHolder.transform);

        // Canvas component ekle
        BuildableCanvas = canvasCreate.AddComponent<Canvas>();
        BuildableCanvas.renderMode = RenderMode.WorldSpace;

        // Canvas Scaler ekle (isteğe bağlı)
        CanvasScaler scaler = canvasCreate.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 1;

        // Graphic Raycaster ekle (isteğe bağlı)
        canvasCreate.AddComponent<GraphicRaycaster>();

        // RectTransform ayarları
        canvasRect = canvasCreate.GetComponent<RectTransform>();
        canvasRect.sizeDelta = canvasSize; // İstediğin boyut
        Canvas = canvasCreate.transform;
    }
    public void GenerateSlots()
    {
        // Öncekileri sil
        foreach (var slot in coinSlots)
        {
            if (slot != null)
                DestroyImmediate(slot);
        }
        coinSlots.Clear();

        int created = 0;
        int row = 0;

        while (created < totalSlots)
        {
            int slotsThisRow = Mathf.Min(maxSlotsPerRow, totalSlots - created);
            float startAngle = -arcAngle / 2f;
            float angleStep = slotsThisRow > 1 ? arcAngle / (slotsThisRow - 1) : 0;
            float radius = fixedRadius - row * rowSpacing;

            for (int i = 0; i < slotsThisRow; i++)
            {
                float angle = startAngle + i * angleStep;
                float rad = Mathf.Deg2Rad * angle;
                Vector3 pos = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0) * radius;

                GameObject slot = Instantiate(slotPrefab, Canvas);
                RectTransform rt = slot.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.localPosition = pos; // anchoredPosition değil!
                }

                coinSlots.Add(slot);
                created++;
            }
            row++;
        }
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (Vector2.Distance(transform.position, CityCenter.Instance.gameObject.transform.position) <= CityCenter.Instance.baseBuildRange)
            centerDistanceCheck = true;
        else
            centerDistanceCheck = false;
        if (collision.gameObject.CompareTag("character") && !isConstruction && centerDistanceCheck)
        {
            foreach (GameObject slots in coinSlots)
            {
                slots.SetActive(true);
            }
        }
        if (collision.gameObject.CompareTag("citizen"))
        {
            if (collision.gameObject.GetComponent<Cconflict>().currentJob == Cconflict.Jobs.Builder)
            {
                currentBuildersOnThis++;
            }
        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("character")&& centerDistanceCheck)
        {
            foreach (GameObject slots in coinSlots)
            {
                slots.SetActive(false);
            }
        }
        if (collision.CompareTag("citizen"))
        {
            Cconflict c = collision.GetComponent<Cconflict>();
            if (c != null && c.currentJob == Cconflict.Jobs.Builder)
            {
                currentBuildersOnThis--;
                currentBuildersOnThis = Mathf.Max(0, currentBuildersOnThis);
            }
        }
    }

    public void PlaceCoin(GameObject coin)
    {
        if (!isConstruction)
        {
            if (currentCoinCount < coinSlots.Count)
            {
                StartCoroutine(MoveCoinToSlot(coin, coinSlots[currentCoinCount].transform));
                coins.Add(coin);
                currentCoinCount++;
            }
        }

    }

    public IEnumerator MoveCoinToSlot(GameObject coin, Transform slot)
    {
        newCoin co = coin.GetComponent<newCoin>();
        Rigidbody2D rbCoin = coin.GetComponent<Rigidbody2D>();
        rbCoin.gravityScale = 0;
        float elapsedTime = 0;
        Vector2 startingPos = coin.transform.position;
        while (elapsedTime < transitionTime)
        {
            coin.transform.position = Vector2.Lerp(startingPos, slot.position, (elapsedTime / transitionTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        coin.transform.position = slot.position;
    }
    public void CheckIfAllSlotsFilled(int currentSlot)
    {
        if (!isConstruction)
        {
            if (!isDroppingCoins)
            {
                StartCoroutine(WaitAndDropCoins(currentSlot));
            }
        }
    }

    public IEnumerator WaitAndDropCoins(int currentSlot)
    {
        isDroppingCoins = true;
        yield return new WaitForSeconds(cr.buildingWaitothcoin); // 2 saniye bekle

        if (currentCoinCount >= coinSlots.Count)
        {
            isDroppingCoins = false;
            yield break;
        }

        if (coinSlots.Count >= currentSlot)
        {
            for (int i = 0; i < coins.Count; i++)
            {
                Rigidbody2D rb = coins[i].GetComponent<Rigidbody2D>();
                rb.gravityScale = newCoinPool.Instance.gravity;
            }
            coins.Clear();
            currentCoinCount = 0;
        }
        isDroppingCoins = false;
    }
    public void DropCoins(int currentSlot)
    {
        currentCoinCount = 0;
        if (coinSlots.Count >= currentSlot)
        {
            for (int i = 0; i < coins.Count; i++)
            {
                Rigidbody2D rb = coins[i].GetComponent<Rigidbody2D>();
                rb.gravityScale = newCoinPool.Instance.gravity;
            }
            coins.Clear();
        }
    }
    public IEnumerator WaitForConstruction()
    {
        yield return new WaitForSeconds(transitionTime + 0.15f);
        StartConstruction();
    }
    public void StartConstruction()
    {
        isConstruction = true;
        sr.sprite = isBuildingSpr;
        tag = "Construction";
        foreach (GameObject slots in coinSlots)
        {
            slots.SetActive(false);
        }
        cr.onBuildable = false;
        currentProgress = 0f;

        currentCoinCount = 0;
        for (int i = 0; i < coins.Count; i++)
        {
            newCoinPool.Instance.DisableCoin(coins[i]);
        }
        coins.Clear();

    }

}
