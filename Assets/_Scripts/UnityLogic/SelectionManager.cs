
public class SelectionManager
{
    private Tile selectedTile;
    public Tile SelectedTile => selectedTile;
    public Piece SelectedPiece => selectedTile?.GetComponent<Tile>()?.Piece.GetComponent<Piece>();

    public void Select(Tile tile)
    {
        selectedTile = tile;
    }

    public void Deselect()
    {
        selectedTile = null;
    }

    public bool HasSelection => selectedTile != null;

}