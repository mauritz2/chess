using UnityEngine;

public class MoveExecutor : MonoBehaviour
{
    [SerializeField] private Board _board;
    [SerializeField] private PieceSetup _pieceSetup;

    public void Execute(MovePlan movePlan)
    {
        switch (movePlan.Kind)
        {
            case MoveKind.Quiet:
                ExecuteQuietMove(movePlan);
                break;
            case MoveKind.Capture:
                ExecuteCaptureMove(movePlan);
                break;
            case MoveKind.Castle:
                ExecuteCastleMove(movePlan);
                break;
            case MoveKind.EnPassant:
                ExecuteCaptureMove(movePlan); // No difference from normal capture
                break;
            case MoveKind.Promotion:
                ExecutePromotionMove(movePlan);
                break;
            default:
                Debug.LogError("Invalid or unhandled move kind: " + movePlan.Kind);
                break;
        }
    }

    private void ExecuteQuietMove(MovePlan movePlan)
    {
        Tile tile = _board.GetTileAt(movePlan.To);
        Tile from = _board.GetTileAt(movePlan.From);
        GameObject piece = from.Piece;
        piece.transform.SetParent(tile.transform);
        piece.transform.localPosition = Vector3.zero;
        tile.Piece = piece;
        from.Piece = null;
    }

    private void ExecuteCaptureMove(MovePlan movePlan)
    {
        // Destroy captured piece
        Tile captureTile = _board.GetTileAt(movePlan.CaptureAt.Value);
        Destroy(captureTile.Piece);
        captureTile.Piece = null;

        // Move the capturing piece
        ExecuteQuietMove(movePlan);
    }

    private void ExecuteCastleMove(MovePlan movePlan)
    {
        // Move the king
        ExecuteQuietMove(movePlan);

        // Move the rook
        MovePlan rookMovePlan = new()
        {
            From = movePlan.RookFrom.Value,
            To = movePlan.RookTo.Value,
        };
        ExecuteQuietMove(rookMovePlan);
    }

    private void ExecutePromotionMove(MovePlan movePlan)
    {
        Tile to = _board.GetTileAt(movePlan.To);
        Tile from = _board.GetTileAt(movePlan.From);

        // Destroy the promoted pawn
        Destroy(from.Piece);
        from.Piece = null;

        // If an opponent piece was captured, destroy it
        if (movePlan.CaptureAt.HasValue)
        {
            Tile captureTile = _board.GetTileAt(movePlan.CaptureAt.Value);
            Destroy(captureTile.Piece);
            captureTile.Piece = null;
        }

        // Place the promoted piece
        _pieceSetup.PlacePiece(movePlan.PromotionPiece, to);
    }
}