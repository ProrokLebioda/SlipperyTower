using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallBounce : MonoBehaviour
{
    private Rigidbody2D rb;
    // Start is called before the first frame update
    Vector2 lastVelocity;
    LineRenderer line;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        line = new LineRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        lastVelocity = rb.velocity;
        Debug.DrawLine(rb.position, rb.position + rb.velocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Wall")
        {
            var speed = lastVelocity.magnitude;
            var direction = Vector2.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
            //rb.AddForce(new Vector2 (direction.x * (speed + 200), direction.y ));
            var bounceForce = (speed * 50);
            if (Mathf.Abs(speed) > 5 && rb.velocity.y > 0)
            {
                //rb.velocity = new Vector2(direction.x * (speed + 80), direction.y + 30);
                rb.velocity = new Vector2(direction.x * (speed + 80), direction.y + 30);
            }
            //else
            //{
            //    rb.velocity
            //}
        }
    }
}
