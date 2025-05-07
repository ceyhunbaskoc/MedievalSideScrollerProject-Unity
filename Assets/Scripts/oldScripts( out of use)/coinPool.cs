using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class coinPool : MonoBehaviour
{
    public int coinCount;
    public GameObject coinPrefab;
    public List<GameObject> coinList=new List<GameObject>();
    private void Start()
    {
        for(int i = 0; i < coinCount; i++)
        {
            GameObject coin = Instantiate(coinPrefab);
            coin.SetActive(false);
            coinList.Add(coin);
        }
    }
    public GameObject GetCoin()
    {
        foreach(GameObject coin in coinList)
        {
            if (coin != null)
            {
                if (!coin.activeInHierarchy)
                {
                    coin.SetActive(true);
                    coin currentcoin = coin.GetComponent<coin>();
                    currentcoin.characterCoin = false;
                    currentcoin.sayac = 0;
                    currentcoin.canRemove = false;
                    return coin;
                }
            }
            
        }
        //Tüm coinler kullanýlýyorsa yeni coin oluþtur
        GameObject newCoin = Instantiate(coinPrefab);
        newCoin.SetActive(true);
        newCoin.GetComponent<coin>().sayac = 0; // Yeni coin için de sýfýrla
        newCoin.GetComponent<coin>().canRemove = false;
        coinList.Add(newCoin);
        return newCoin;
    }
    public void DisableCoin(GameObject coin)
    {
        coin.SetActive(false );

    }

}
