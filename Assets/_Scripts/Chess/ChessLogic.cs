using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChessLogic : MonoBehaviour
{
    public List<Vector2Int> GetValidMoves(Vector2Int from,
                                         Piece[,] boardState,
                                         Player player,
                                         Vector2Int? enPasssantTargetPos)
    {
        Piece piece = boardState[from.x, from.y];
        List<Vector2Int> normalMoves = GetNormalMoves(from, boardState, player);
        Vector2Int? enPassantMove = GetEnPassantMove(from, enPasssantTargetPos, boardState, player);
        List<Vector2Int> castlingMoves = GetCastlingMoves(piece, boardState, player);
        List<Vector2Int> allMoves = normalMoves.Concat(castlingMoves).ToList();
        if (enPassantMove.HasValue) allMoves.Add(enPassantMove.Value);
        return allMoves;
    }

    public List<Vector2Int> GetNormalMoves(Vector2Int from,
                                         Piece[,] boardState,
                                         Player player)
    {
        List<Vector2Int> legalMoves = new();
        Piece p = boardState[from.x, from.y];

        return p.
                GetPossibleMoves(from, boardState)
                .Where(move =>
                {
                    Piece[,] simulatedBoardState = (Piece[,])boardState.Clone();
                    simulatedBoardState[move.x, move.y] = boardState[from.x, from.y];
                    simulatedBoardState[from.x, from.y] = null;
                    return !IsKingInCheck(simulatedBoardState, player);
                })
                .ToList();
    }

    public Vector2Int? GetEnPassantMove(Vector2Int from, Vector2Int? enPassantTargetPos, Piece[,] boardState, Player player)
    {
        if (!enPassantTargetPos.HasValue)
        {
            return null;
        }

        Vector2Int to = enPassantTargetPos.Value;
        int dx = to.x - from.x;
        int dy = to.y - from.y;

        if (Mathf.Abs(dx) != 1 || Mathf.Abs(dy) != 1)
        {
            return null;
        }

        Piece[,] simulatedBoardState = (Piece[,])boardState.Clone();
        simulatedBoardState[to.x, to.y] = boardState[from.x, from.y];
        simulatedBoardState[from.x, from.y] = null;
        if (IsKingInCheck(simulatedBoardState, player))
        {
            return null;
        }

        return enPassantTargetPos;

    }

    public List<Vector2Int> GetCastlingMoves(Piece piece, Piece[,] boardState, Player player)
    {
        List<Vector2Int> castlingMoves = new();

        if (piece.Type != PieceType.King || piece.hasMoved)
            return castlingMoves;

        // Note that order matters here due to path.Take() later in this function - the king's square should be last 
        var pathsToCheck = new Dictionary<string, Dictionary<Player, List<Vector2Int>>>
        {
            ["long"] = new()
            {
                { Player.White, new() { new(1,0), new(2,0), new(3,0), new(4,0) } },
                { Player.Black, new() { new(1,7), new(2,7), new(3,7), new(4,7) } }
            },
            ["short"] = new()
            {
                { Player.White, new() { new(5,0), new(6,0), new(4,0) } },
                { Player.Black, new() { new(5,7), new(6,7), new(4,7) } }
            }
        };

        // Initiate check if long or short castling is allowed
        foreach (var side in new[] { "long", "short" })
        {
            Vector2Int rookCoord = Constants.rookCastlingFrom[side][player];
            Piece rook = boardState[rookCoord.x, rookCoord.y];

            bool rookReady = rook != null && rook.Type == PieceType.Rook && !rook.hasMoved;

            List<Vector2Int> path = pathsToCheck[side][player];
            bool pathClear = path
                .Take(side == "long" ? 3 : 2) // Only middle tiles must be empty
                .All(coord => boardState[coord.x, coord.y] == null);

            bool kingPathSafe = path
                .All(coord => !IsTileAttacked(coord, boardState, player));

            if (rookReady && pathClear && kingPathSafe)
            {
                castlingMoves.Add(Constants.kingCastlingTo[side][player]);
            }
        }
        return castlingMoves;
    }

    public bool IsKingInCheck(Piece[,] boardState, Player attacked)
    {
        // TODO - refactor to use IsTileAttacked instead
        int width = Constants.BoardWidth;
        int height = Constants.BoardHeight;

        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                Piece p = boardState[x, y];
                if (p != null && p.Type == PieceType.King && p.Player == attacked)
                {
                    Vector2Int kingCoordinates = new(x, y);
                    return IsTileAttacked(kingCoordinates, boardState, attacked);
                }
            }
        }
        return false;
    }

    public bool IsTileAttacked(Vector2Int coordinates, Piece[,] boardState, Player attacked)
    {
        int width = Constants.BoardWidth;
        int height = Constants.BoardHeight;

        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                Piece p = boardState[x, y];
                if (p != null && p.Player != attacked)
                {
                    var moves = p.GetPossibleMoves(new Vector2Int(x, y), boardState);
                    if (moves.Contains(coordinates))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool IsInCheckmate(Piece[,] boardState, Player victim, Vector2Int? enPassantTargetPos)
    {
        for (int x = 0; x < Constants.BoardWidth; x++)
        {
            for (int y = 0; y < Constants.BoardHeight; y++)
            {
                Piece piece = boardState[x, y];
                if (piece != null && piece.Player == victim)
                {
                    List<Vector2Int> moves = GetValidMoves(
                        new Vector2Int(x, y),
                        boardState,
                        victim,
                        enPassantTargetPos
                    );

                    bool hasValidMove = moves.Count > 0;

                    if (hasValidMove)
                    {
                        return false;
                    }
                }
            }
        }

        return true;

    }
    

}