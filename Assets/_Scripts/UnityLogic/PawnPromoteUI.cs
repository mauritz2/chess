using System;
using UnityEngine;

public class PawnPromoteUI : MonoBehaviour
{
    // TODO - merge with GameUI
    public Action<PieceType> OnPieceSelected;

    public void SelectPiece(string pieceTypeStr)
    {
        PieceType pieceType;
        switch (pieceTypeStr)
        {
            case "Queen":
                pieceType = PieceType.Queen;
                break;
            case "Rook":
                pieceType = PieceType.Rook;
                break;
            case "Bishop":
                pieceType = PieceType.Bishop;
                break;
            case "Knight":
                pieceType = PieceType.Knight;
                break;
            default:
                throw new ArgumentException($"Unknown piece type: {pieceTypeStr}");
        }
        OnPieceSelected?.Invoke(pieceType);
    }
}