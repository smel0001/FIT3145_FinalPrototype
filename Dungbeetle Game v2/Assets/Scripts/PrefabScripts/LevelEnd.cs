using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Respawn playerRespawn = col.gameObject.GetComponent<Respawn>();
            playerRespawn.ToMenu();
        }
    }
}