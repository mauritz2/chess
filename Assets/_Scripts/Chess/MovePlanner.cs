using System.Collections.Generic;
using UnityEngine;

public sealed class MovePlanner
{
    private readonly ChessLogic _logic;
    public MovePlanner(ChessLogic logic) => _logic = logic;
    public MovePlan Plan(Vector2Int from, Vector2Int to, Piece[,] boardState, Player player, Vector2Int? enPassantTargetPos)
    {
        Piece piece = boardState[from.x, from.y].GetComponent<Piece>();

        MovePlan plan = new()
        {
            Kind = MoveKind.Invalid,
            From = from,
            To = to,
            CaptureAt = null,
            RookFrom = null,
            RookTo = null,
            PromotionPiece = null
        };

        List<Vector2Int> castlingMoves = _logic.GetCastlingMoves(piece, boardState, player);
        if (castlingMoves.Contains(to))
        {
            plan.Kind = MoveKind.Castle;

            if (to.x == Constants.kingCastlingTo["long"][player].x)
            {
                plan.RookFrom = Constants.rookCastlingFrom["long"][player];
                plan.RookTo = Constants.rookCastlingTo["long"][player];
            }
            else if (to.x == Constants.kingCastlingTo["short"][player].x)
            {
                plan.RookFrom = Constants.rookCastlingFrom["short"][player];
                plan.RookTo = Constants.rookCastlingTo["short"][player];
            }

            return plan;
        }


        Vector2Int? enPassantMove = _logic.GetEnPassantMove(from, enPassantTargetPos, boardState, player);
        if (enPassantMove.HasValue && enPassantMove.Value == to)
        {
            plan.Kind = MoveKind.EnPassant;
            plan.CaptureAt = new Vector2Int(to.x, to.y - Constants.Step[player]);
            return plan;
        }
        
        int endRank = player == Player.White ? 7 : 0;
        if (piece.Type == PieceType.Pawn && to.y == endRank)
        {
            plan.Kind = MoveKind.Promotion;
            if (boardState[to.x, to.y] != null && boardState[to.x, to.y].Player != player)
            {
                plan.CaptureAt = to;
                return plan;
            }
            return plan;
        }

        List<Vector2Int> normalMoves = _logic.GetNormalMoves(from, boardState, player);
        if (normalMoves.Contains(to))
        {
            if (boardState[to.x, to.y] != null && boardState[to.x, to.y].Player != player)
            {
                plan.Kind = MoveKind.Capture;
                plan.CaptureAt = to;
                return plan;
            }
            else
            {
                plan.Kind = MoveKind.Quiet;
                return plan;
            }
        }

        return plan;               
    }
}