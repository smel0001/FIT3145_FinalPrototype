using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {

            //when player collides, pass position to respawn

            Respawn playerRespawn = col.gameObject.GetComponent<Respawn>();
            playerRespawn.checkPointTrigger(transform.position);
        }
    }
}
