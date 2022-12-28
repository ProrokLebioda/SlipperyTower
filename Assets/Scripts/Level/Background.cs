using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    [SerializeField]
    private float backgroundHeight;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2(spriteRenderer.size.x, backgroundHeight);
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.offset = new Vector2(boxCollider.offset.x ,Mathf.Ceil(spriteRenderer.size.y / 2));

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void PlaceAtPosition(Vector2 position)
    {
        // add enough to place background based on fact we expand Background from the middle
        Vector2 size = spriteRenderer.size;
        Vector2 sizeRounded = new Vector2(0, Mathf.Ceil(size.y / 2));
        transform.position = position + sizeRounded;
    }
}
