using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    [SerializeField]
    private float _backgroundHeight;


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.size = new Vector2(_spriteRenderer.size.x, _backgroundHeight);
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.offset = new Vector2(_boxCollider.offset.x ,Mathf.Ceil(_spriteRenderer.size.y / 2));
    }

    public void PlaceAtPosition(Vector2 position)
    {
        Vector2 size = _spriteRenderer.size;
        // add enough to place background based on fact we expand Background from the middle
        Vector2 sizeRounded = new Vector2(0, Mathf.Ceil(size.y / 2));
        transform.position = position + sizeRounded;
    }
}
