using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMovingPlatform : MonoBehaviour
{
    public float speed = 2;
    public float moveDist = 5;

    private Vector3 startPosition;
    private bool movingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingRight)
        {
            if (transform.position.x <= startPosition.x + moveDist)
            {
                transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
            }
            else
            {
                movingRight = false;
            }
        }
        else
        {
            if (transform.position.x >= startPosition.x)
            {
                transform.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
            }
            else
            {
                movingRight = true;
            }
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            float vel = speed;
            if (!movingRight)
            {
                vel *= -1;
            }

            PlayerController playerController = col.gameObject.GetComponent<PlayerController>();
            playerController.AddHorizontalForce(vel);
        }
    }
}

