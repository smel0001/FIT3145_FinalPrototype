using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerMove playerMove = col.gameObject.GetComponent<PlayerMove>();

            playerMove.deceleration = 4.0f;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerMove playerMove = col.gameObject.GetComponent<PlayerMove>();

            playerMove.deceleration = 40.0f;
        }
    }
}
