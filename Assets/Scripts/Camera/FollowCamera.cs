using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    private Transform lookAt;
    public float boundY = 5f;
    private bool isMoving = false;
    public float cameraSpeed = 0.2f;

    private void Start()
    {
        lookAt = GameObject.Find("Player").transform;
    }

    private void LateUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        if (lookAt)
        {
            Vector3 delta = Vector3.zero;

            // This is to check if we're inside the bounds on the Y axis
            float deltaY = lookAt.position.y - transform.position.y;
            if (deltaY > boundY || deltaY < -boundY)
            {
                if (transform.position.y < lookAt.position.y)
                {
                    delta.y = deltaY - boundY;
                    isMoving = true;
                }
                //else
                //{
                //    delta.y = deltaY + boundY;
                //}
            }
            if (isMoving)
            {
                transform.position += new Vector3(0, delta.y + (cameraSpeed * Time.deltaTime), 0);
            }
            else
            {
                transform.position += new Vector3(0, delta.y, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {

            int floor = Mathf.FloorToInt(GameObject.Find("Player").GetComponent<PlayerWallBounce>().CurrentPlayerHeight);
            GameManager.Instance.PlayerFloor = floor;
            lookAt = null;
            Destroy(collision.gameObject);
            GameManager.Instance.OnPlayerDeath();
        }
        else if (collision.tag == "Platform")
        {
            Destroy(collision.gameObject); // put in object pool maybe?
        }
    }

    
}
