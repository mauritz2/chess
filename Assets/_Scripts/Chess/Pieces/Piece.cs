using UnityEngine;
using System.Collections.Generic;

public abstract class Piece : MonoBehaviour
{
    public Player Player;
    public PieceType Type;
    public bool hasMoved = false;
    public int Step => Constants.Step[Player]; // reminder => is a read-only property that computes its value dynamically
    public abstract List<Vector2Int> GetPossibleMoves(Vector2Int coordinates, Piece[,] allPieces);

    public List<Vector2Int> GetHorizontalAndVerticalMoves(Vector2Int pos, Piece[,] boardState)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new (0, 1), new (0, -1),
            new (1, 0), new (-1, 0)
        };

        return GetMoves(pos, directions, boardState);
    }
    public List<Vector2Int> GetDiagonalMoves(Vector2Int pos, Piece[,] boardState)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new (1, 1), new (-1, -1),
            new (1, -1), new (-1, 1)
        };

        return GetMoves(pos, directions, boardState);
    }

    public List<Vector2Int> GetMoves(Vector2Int pos, Vector2Int[] directions, Piece[,] boardState)
    {
        List<Vector2Int> moves = new();

        foreach (var dir in directions)
        {
            for (int step = 1; step < Constants.BoardHeight; step++)
            {
                var newPos = pos + dir * step;
                if (IsOutOfBounds(newPos))
                    break;

                var piece = boardState[newPos.x, newPos.y];
                if (piece == null)
                {
                    moves.Add(newPos);
                    continue;
                }

                if (piece.Player != Player)
                    moves.Add(newPos);

                break;
            }
        }

        return moves;
    }
    
    public bool IsOutOfBounds(Vector2Int pos)
    {
        int width = Constants.BoardWidth;
        int height = Constants.BoardHeight;
        bool isOutOfBounds = pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height;
        return isOutOfBounds;
    }
}
