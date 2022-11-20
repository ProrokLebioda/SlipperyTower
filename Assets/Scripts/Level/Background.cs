using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
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
