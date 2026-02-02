using UnityEngine;
using DG.Tweening;

public class Block : MonoBehaviour
{
    public int x { get; private set; }
    public int y { get; private set; }
    public int colorIndex { get; private set; }

    private SpriteRenderer spriteRenderer;

    // Cache the initial scale to prevent resizing issues when reusing blocks from the pool
    private Vector3 defaultScale;

    private void Awake()
    {
        // Capture the scale set in the Inspector (e.g., 0.9 for spacing)
        defaultScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(int x, int y, int colorIndex, Sprite startSprite)
    {
        this.x = x;
        this.y = y;
        this.colorIndex = colorIndex;
        gameObject.name = $"Block [{x},{y}]";

        if (startSprite != null)
        {
            spriteRenderer.sprite = startSprite;
        }
    }

    public void MoveToPosition(Vector2 targetPos, float duration = 0.3f)
    {
        transform.DOKill(); // Kill any active tweens to avoid conflicts
        transform.DOMove(targetPos, duration).SetEase(Ease.OutQuad);
    }

    public void PlaySpawnAnimation()
    {
        // Reset scale to zero and tween to defaultScale for a pop-up effect
        transform.localScale = Vector3.zero;
        transform.DOScale(defaultScale, 0.3f).SetEase(Ease.OutBack);
    }

    public void PlayDestroyAnimation()
    {
        // Disable collider immediately to prevent double-clicking
        GetComponent<Collider2D>().enabled = false;

        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            // Reset state for pooling
            GetComponent<Collider2D>().enabled = true;
            transform.localScale = defaultScale; // Restore original size

            // Return to pool instead of destroying
            BlockPool.Instance.ReturnBlock(this);
        });
    }
}
