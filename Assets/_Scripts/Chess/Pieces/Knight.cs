using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    void Start()
    {
        Type = PieceType.Knight;
    }
    public override List<Vector2Int> GetPossibleMoves(Vector2Int pos, Piece[,] boardState)
    {
        List<Vector2Int> moves = new();
        List<Vector2Int> knightMoves = new()
        {
            new( 1,  2), new(-1,  2),
            new( 2,  1), new( 2, -1),
            new(-2,  1), new(-2, -1),
            new(-1, -2), new( 1, -2)
        };

        foreach (var move in knightMoves)
        {
            Vector2Int newPos = pos + move;
            if (IsOutOfBounds(newPos)) continue;
            Piece p = boardState[newPos.x, newPos.y];
            if (p != null && p.Player == this.Player) continue;
            moves.Add(newPos);
        }

        return moves;

    }
}
