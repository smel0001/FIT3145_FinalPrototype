using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private CoinManager coinManager;

    // Start is called before the first frame update
    void Start()
    {
        coinManager = FindObjectOfType<CoinManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            coinManager.CollectedCoin();
        }
    }
}