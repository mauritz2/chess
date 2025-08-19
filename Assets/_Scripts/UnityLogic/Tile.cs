using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor1;
    [SerializeField] private SpriteRenderer _renderer;

    private bool _isOffset;

    public GameObject MoveHighlight, CheckHighlight, AttackHighlight;
    public GameObject Piece;

    public void Init(bool isOffset)
    {
        _isOffset = isOffset;
        _renderer.color = isOffset ? _offsetColor1 : _baseColor;
    }

    void OnMouseDown()
    {
        GameManager.Instance.OnTileClicked(this);
    }
}
