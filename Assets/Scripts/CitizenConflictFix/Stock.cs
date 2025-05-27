using System.Collections;
using TMPro;
using UnityEngine;
using static Cconflict;

public class Stock : buildableObject
{
    public enum StockType { hammer,bow}
    public StockType currentType;
    public TextMeshProUGUI stockText;

    void Update()
    {
        if (coins.Count == coinSlots.Count)
        {
            ClearCoinSlots();
        }
        if (currentType == StockType.bow)
            stockText.text = GameManager.Instance.bowStock.ToString();
        if (currentType == StockType.hammer)
            stockText.text = GameManager.Instance.hammerStock.ToString();

    }
    public void ClearCoinSlots()
    {
        currentCoinCount = 0;
        if (currentType == StockType.bow)
            GameManager.Instance.bowStock++;
        if (currentType == StockType.hammer)
            GameManager.Instance.hammerStock++;
        for (int i = 0; i < coins.Count; i++)
        {
            newCoinPool.Instance.DisableCoin(coins[i]);
        }
        coins.Clear();
    }
    public override void PlaceCoin(GameObject coin)
    {
        //print("PLACECOIN ÇALIÞTI");
        if (!isConstruction)
        {
            if (currentCoinCount < coinSlots.Count)
            {
                StartCoroutine(MoveCoinToSlot(coin, coinSlots[currentCoinCount].transform));
                currentCoinCount++;
            }
        }

    }

    public override IEnumerator MoveCoinToSlot(GameObject coin, Transform slot)
    {
        //print("movecointoslot");
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
        coins.Add(coin);
    }

}
