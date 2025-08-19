using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class King : Piece
{
    void Start()
    {
        Type = PieceType.King;
    }

    public override List<Vector2Int> GetPossibleMoves(Vector2Int pos, Piece[,] boardState)
    {
        List<Vector2Int> moves = new();
        List<int> directions = Enumerable.Range(-1, 3).ToList();
        foreach (int x in directions)
        {
            foreach (int y in directions)
            {
                Vector2Int newPos = new(pos.x + x, pos.y + y);
                bool validMove = !IsOutOfBounds(newPos) &&
                                (boardState[newPos.x, newPos.y] == null ||
                                 boardState[newPos.x, newPos.y].Player != this.Player);
                if (validMove) moves.Add(newPos);
            }
        }
        return moves;
    }
}