using UnityEngine;
using System.Collections.Generic;

public class Bishop : Piece
{
    void Start()
    {
        Type = PieceType.Bishop;
    }
    public override List<Vector2Int> GetPossibleMoves(Vector2Int pos, Piece[,] boardState)
    {
        return GetDiagonalMoves(pos, boardState);
    }
}
