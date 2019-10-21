using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineBlock : MonoBehaviour
{
    public float playervelocityY = 15.0f;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = col.gameObject.GetComponent<PlayerController>();
            playerController.SetVerticalVelocity(playervelocityY);
        }
    }
}
