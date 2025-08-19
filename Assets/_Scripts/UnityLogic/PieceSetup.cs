using UnityEngine;

public class PieceSetup : MonoBehaviour
{
    [SerializeField] private GameObject _kingW, _queenW, _rookW, _knightW, _bishopW, _pawnW;
    [SerializeField] private GameObject _kingB, _queenB, _rookB, _knightB, _bishopB, _pawnB;

    public void SetupPieces(Board board)
    {
        PieceType[] backRankOrder = {
            PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen,
            PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook
        };

        for (int x = 0; x < board.Width; x++)
        {
            // White pieces
            PlacePiece(GetPiecePrefab(backRankOrder[x], Player.White), board.GetTileAt(new Vector2Int(x, 0)));
            PlacePiece(GetPiecePrefab(PieceType.Pawn, Player.White), board.GetTileAt(new Vector2Int(x, 1)));

            // Black pieces
            PlacePiece(GetPiecePrefab(PieceType.Pawn, Player.Black), board.GetTileAt(new Vector2Int(x, 6)));
            PlacePiece(GetPiecePrefab(backRankOrder[x], Player.Black), board.GetTileAt(new Vector2Int(x, 7)));
        }
    }

    public GameObject GetPiecePrefab(PieceType type, Player player)
    {
        return type switch
        {
            PieceType.Pawn => player == Player.White ? _pawnW : _pawnB,
            PieceType.Rook => player == Player.White ? _rookW : _rookB,
            PieceType.Knight => player == Player.White ? _knightW : _knightB,
            PieceType.Bishop => player == Player.White ? _bishopW : _bishopB,
            PieceType.Queen => player == Player.White ? _queenW : _queenB,
            PieceType.King => player == Player.White ? _kingW : _kingB,
            _ => throw new System.ArgumentException($"Unknown piece type: {type}")
        };
    }

    public void PlacePiece(GameObject piecePrefab, Tile tile)
    {
        GameObject piece = Instantiate(piecePrefab, tile.transform);
        piece.transform.localPosition = Vector3.zero;
        tile.Piece = piece;
    }


}