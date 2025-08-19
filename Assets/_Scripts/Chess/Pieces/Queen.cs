using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Queen : Piece
{
    void Start()
    {
        Type = PieceType.Queen;
    }
    public override List<Vector2Int> GetPossibleMoves(Vector2Int pos, Piece[,] boardState)
    {
        List<Vector2Int> horizontalMoves = GetHorizontalAndVerticalMoves(pos, boardState);
        List<Vector2Int> diagonalMoves = GetDiagonalMoves(pos, boardState);
        return horizontalMoves.Concat(diagonalMoves).ToList();
    }
}
