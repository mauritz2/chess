using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PieceTest
{
    [Test]
    public void TestQueenMovement_WhenBlocked()
    {
        Piece[,] board = BoardTestFactory.CreateEmptyBoard();
        Piece queen = BoardTestFactory.CreatePiece(PieceType.Queen, Player.White);
        Piece friendlyBlocker = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.White);
        Piece opponentBlocker = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.Black);

        board[1, 1] = queen;
        board[2, 2] = friendlyBlocker;
        board[0, 0] = opponentBlocker;

        List<Vector2Int> moves = queen.GetPossibleMoves(new Vector2Int(1, 1), board);
        Assert.AreEqual(17, moves.Count);
        Assert.IsFalse(moves.Contains(new Vector2Int(2, 2)));
        Assert.Contains(new Vector2Int(0, 0), moves);
    }

    [Test]
    public void TestRookMovement_WhenBlocked()
    {
        Piece[,] board = BoardTestFactory.CreateEmptyBoard();
        Piece rook = BoardTestFactory.CreatePiece(PieceType.Rook, Player.White);
        Piece friendlyBlocker = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.White);
        Piece opponentBlocker = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.Black);

        board[1, 1] = rook;
        board[0, 1] = friendlyBlocker;
        board[2, 1] = opponentBlocker;

        List<Vector2Int> moves = rook.GetPossibleMoves(new Vector2Int(1, 1), board);
        Assert.AreEqual(8, moves.Count);
        Assert.IsFalse(moves.Contains(new Vector2Int(0, 1)));
        Assert.Contains(new Vector2Int(2, 1), moves);
    }

    [Test]
    public void TestBishopMovement_WhenBlocked()
    {
        Piece[,] board = BoardTestFactory.CreateEmptyBoard();
        Piece bishop = BoardTestFactory.CreatePiece(PieceType.Bishop, Player.White);
        Piece friendlyBlocker = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.White);
        Piece opponentBlocker = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.Black);

        board[1, 1] = bishop;
        board[2, 2] = friendlyBlocker;
        board[0, 0] = opponentBlocker;

        List<Vector2Int> moves = bishop.GetPossibleMoves(new Vector2Int(1, 1), board);
        Assert.AreEqual(3, moves.Count);
        Assert.IsFalse(moves.Contains(new Vector2Int(2, 2)));
        Assert.Contains(new Vector2Int(0, 0), moves);
    }

    [Test]
    public void TestKnightMovement_WhenBlocked()
    {
        Piece[,] board = BoardTestFactory.CreateEmptyBoard();
        Piece knight = BoardTestFactory.CreatePiece(PieceType.Knight, Player.White);
        Piece friendlyBlocker = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.White);
        Piece opponentBlocker = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.Black);

        board[1, 1] = knight;
        board[2, 3] = friendlyBlocker;
        board[3, 2] = opponentBlocker;

        List<Vector2Int> moves = knight.GetPossibleMoves(new Vector2Int(1, 1), board);
        Assert.AreEqual(3, moves.Count);
        Assert.IsFalse(moves.Contains(new Vector2Int(2, 3)));
        Assert.Contains(new Vector2Int(3, 2), moves);
    }

    [Test]
    public void TestKingMovement_WhenBlocked()
    {
        Piece[,] board = BoardTestFactory.CreateEmptyBoard();
        Piece king = BoardTestFactory.CreatePiece(PieceType.King, Player.White);
        Piece friendlyBlocker = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.White);
        Piece opponentBlocker = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.Black);

        board[1, 1] = king;
        board[0, 0] = friendlyBlocker;
        board[2, 2] = opponentBlocker;

        List<Vector2Int> moves = king.GetPossibleMoves(new Vector2Int(1, 1), board);
        Assert.AreEqual(7, moves.Count);
        Assert.IsFalse(moves.Contains(new Vector2Int(0, 0)));
        Assert.Contains(new Vector2Int(2, 2), moves);
    }

        [Test]
    public void TestPawnDoubleMove_WhenHaventMoved()
    {
        Vector2Int pawnPos = new Vector2Int(1, 1);
        var pawn = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.White);
        var board = BoardTestFactory.CreateBoardWithPiece(pawnPos, pawn);

        List<Vector2Int> moves = pawn.GetPossibleMoves(pawnPos, board);

        Assert.AreEqual(2, moves.Count);
        Assert.Contains(new Vector2Int(1, 3), moves);
        Assert.Contains(new Vector2Int(1, 2), moves);   
    }

    [Test]
    public void TestPawnDoubleMove_WhenHaveMoved()
    {
        Vector2Int pawnPos = new (1, 1);
        var pawn = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.White, true);
        var board = BoardTestFactory.CreateBoardWithPiece(pawnPos, pawn);
        List<Vector2Int> moves = pawn.GetPossibleMoves(pawnPos, board);
        Assert.AreEqual(1, moves.Count);
        Assert.Contains(new Vector2Int(1, 2), moves);   
    }

    [Test]
    public void TestPawnMovement_WhenBlocked()
    {
        Vector2Int pawnPos = new(1, 1);
        var pawn = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.White);
        var board = BoardTestFactory.CreateBoardWithPiece(pawnPos, pawn);

        board[pawnPos.x, pawnPos.y + 1] = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.White);
        List<Vector2Int> moves = pawn.GetPossibleMoves(pawnPos, board);

        Assert.AreEqual(0, moves.Count);
    }

    [Test]
    public void TestMovement_GoingOutOfBounds()
    {
        Vector2Int pawnPos = new(0, 7);
        var pawn = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.White);
        var board = BoardTestFactory.CreateBoardWithPiece(pawnPos, pawn);
        List<Vector2Int> moves = pawn.GetPossibleMoves(pawnPos, board);
        Assert.AreEqual(0, moves.Count);
    }

    [Test]
    public void TestPawnCapture_WhenCaptureAvailable()
    {
        Vector2Int pawnPos = new (4, 4);

        List<Player> players = new() { Player.White, Player.Black };
        foreach (Player p in players)
        {
            var pawn = BoardTestFactory.CreatePiece(PieceType.Pawn, p, true);
            var board = BoardTestFactory.CreateBoardWithPiece(pawnPos, pawn);

            int dir = p == Player.White ? 1 : -1;
            List<Vector2Int> toCheck = new()
            {
                new (pawnPos.x - 1, pawnPos.y + dir),
                new (pawnPos.y + 1, pawnPos.y + dir),
            };

            foreach (Vector2Int c in toCheck)
            {
                Player opponent = p == Player.White ? Player.Black : Player.White;
                board[c.x, c.y] = BoardTestFactory.CreatePiece(PieceType.Pawn, opponent);
                List<Vector2Int> moves = pawn.GetPossibleMoves(pawnPos, board);
                Assert.AreEqual(2, moves.Count);
                Assert.Contains(c, moves);
                board[c.x, c.y] = null;
            }
        }
    }
}
