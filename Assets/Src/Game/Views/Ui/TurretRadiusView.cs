using UnityEngine;

public class TurretRadiusView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;

    public void SetSize(float size)
    {
        var currentSize = _sprite.bounds.size.x;
        SetScale(size / currentSize);
    }

    private void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, 1, scale);
    }
}
