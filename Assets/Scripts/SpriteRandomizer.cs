using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRandomizer : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if(sprites.Count > 0)
            _spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];
    }
}
