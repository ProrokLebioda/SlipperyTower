using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallBounce : MonoBehaviour
{
    private Rigidbody2D rb;
    public GameObject level0;
    public GameObject playerGroundCheck;
    
    private float bounceCooldown = 5.0f;
    private float timeSinceLastBounce=0;

    private float currentPlayerHeight;

    // Start is called before the first frame update
    Vector2 lastVelocity;
    LineRenderer line;

    public float CurrentPlayerHeight { get => currentPlayerHeight; set => currentPlayerHeight = value; }

    void Start()
    {
        timeSinceLastBounce = -bounceCooldown; //beacuse we check delta of currentFill time and time since last compared to cooldown, we want to be larger than cooldown in first few seconds
        rb = GetComponent<Rigidbody2D>();
        line = new LineRenderer();
        CurrentPlayerHeight = level0.transform.position.y;
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
            if (Time.time - timeSinceLastBounce > bounceCooldown)
            {
                var speed = lastVelocity.magnitude;
                var direction = Vector2.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
                //rb.AddForce(new Vector2 (direction.x * (speed + 200), direction.y ));
                if (Mathf.Abs(speed) > 5 && rb.velocity.y > 0)
                {
                    var bounceForce = (speed*2);
                    //rb.velocity = new Vector2(direction.x * (speed + 80), direction.y + 30);
                    rb.velocity = new Vector2(direction.x * bounceForce, direction.y * bounceForce); // doesn't feel right
                    timeSinceLastBounce = Time.time;
                }
            }
            //else
            //{
            //    rb.velocity
            //}
        }
        if (collision.collider.tag == "Platform")
        {
            if (rb.velocity.normalized.y <= 0.0f && GetComponent<CharacterController2D>().IsGrounded() )
            {
                float platformHeight = collision.collider.gameObject.transform.position.y;
                if (platformHeight > level0.transform.position.y)
                {
                    if (CurrentPlayerHeight < platformHeight)
                    {
                        float deltaHeight = platformHeight - CurrentPlayerHeight;
                        CurrentPlayerHeight = platformHeight;
                        GameManager.Instance.UpdateScore(Mathf.CeilToInt(deltaHeight));

                    }
                }
            }
        }
    }

    // doesn't work perfectly, sometimes slow jumps are not recognised
    public void LandedOnPlatform()
    {
        timeSinceLastBounce = -bounceCooldown;
        //if (playerGroundCheck.transform.position.y > level0.transform.position.y && rb.velocity.normalized.y <= 0.0f)
        //{
        //    float deltaHeight = playerGroundCheck.transform.position.y - currentPlayerHeight;
        //    if (deltaHeight > 0.2f)
        //    {
        //        currentPlayerHeight = playerGroundCheck.transform.position.y;
        //        GameManager.Instance.UpdateScore(Mathf.CeilToInt(deltaHeight));
        //    }
        //}
    }
}
