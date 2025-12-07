using UnityEngine;

public class CardSprite : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    public bool isFaceUp = false;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isFaceUp)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
    }
}
