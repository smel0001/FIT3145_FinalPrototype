using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YMovingPlatform : MonoBehaviour
{
    public float speed = 2;
    public float moveDist = 5;

    private Vector3 startPosition;
    private bool movingUp = true;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingUp)
        {
            if (transform.position.y <= startPosition.y + moveDist)
            {
                transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
            }
            else
            {
                movingUp = false;
            }
        }
        else
        {
            if (transform.position.y >= startPosition.y)
            {
                transform.position -= new Vector3(0f, speed * Time.deltaTime, 0f);
            }
            else
            {
                movingUp = true;
            }
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            float vel = speed;
            if (!movingUp)
            {
                vel *= -1;
            }

            PlayerController playerController = col.gameObject.GetComponent<PlayerController>();
            playerController.AddVerticalForce(vel);
        }
    }
}
