using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public int MaxCoins = 3;
    private int curCoins;
    public Text coins;
    // Start is called before the first frame update
    void Start()
    {
        curCoins = 0;
    }

    // Update is called once per frame
    void Update()
    {
        coins.text = "Coins: " + curCoins + " / " + MaxCoins;
    }

    public void CollectedCoin()
    {
        curCoins++;
    }
}
