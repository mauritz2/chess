using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ChessLogicTest
{

    private ChessLogic _chessLogic;

    public ChessLogicTest()
    {
        _chessLogic = new ChessLogic();
    }

    [Test]
    public void TestCastlingMove_WhenNoPiecesMoved()
    {
        foreach (Player player in Constants.Players)
        {
            Piece[,] boardState = BoardTestFactory.CreateCastlingScenario(player);

            Vector2Int kingCoord = player == Player.White ? new Vector2Int(4, 0) : new Vector2Int(4, 7);
            Piece king = boardState[kingCoord.x, kingCoord.y];

            List<Vector2Int> castlingMoves = _chessLogic.GetCastlingMoves(king, boardState, player);
            Assert.IsTrue(castlingMoves.Contains(Constants.kingCastlingTo["short"][player]));
            Assert.IsTrue(castlingMoves.Contains(Constants.kingCastlingTo["long"][player]));
        }
    }

    [Test]
    public void TestCastlingMove_WhenKingHasMoved()
    {
        foreach (Player player in Constants.Players)
        {
            Piece[,] boardState = BoardTestFactory.CreateCastlingScenario(player, true, false);

            Vector2Int kingCoord = player == Player.White ? new Vector2Int(4, 0) : new Vector2Int(4, 7);
            Piece king = boardState[kingCoord.x, kingCoord.y];

            List<Vector2Int> castlingMoves = _chessLogic.GetCastlingMoves(king, boardState, player);
            Assert.AreEqual(castlingMoves.Count, 0);
        }
    }

    [Test]
    public void TestCastlingMove_WhenRooksHaveMoved()
    {
        foreach (Player player in Constants.Players)
        {
            Piece[,] boardState = BoardTestFactory.CreateCastlingScenario(player, false, true);

            Vector2Int kingCoord = player == Player.White ? new Vector2Int(4, 0) : new Vector2Int(4, 7);
            Piece king = boardState[kingCoord.x, kingCoord.y];

            List<Vector2Int> castlingMoves = _chessLogic.GetCastlingMoves(king, boardState, player);
            Assert.AreEqual(castlingMoves.Count, 0);
        }
    }

    [Test]
    public void TestCastlingMove_WhenPieceBlocking()
    {
        foreach (Player player in Constants.Players)
        {
            Piece[,] boardState = BoardTestFactory.CreateCastlingScenario(player);

            Piece bishop = BoardTestFactory.CreatePiece(PieceType.Bishop, player);
            Vector2Int blockingPosition = player == Player.White ? new Vector2Int(5, 0) : new Vector2Int(5, 7);
            boardState[blockingPosition.x, blockingPosition.y] = bishop;

            Vector2Int kingCoord = player == Player.White ? new Vector2Int(4, 0) : new Vector2Int(4, 7);
            Piece king = boardState[kingCoord.x, kingCoord.y];

            List<Vector2Int> castlingMoves = _chessLogic.GetCastlingMoves(king, boardState, player);

            Assert.IsTrue(castlingMoves.Contains(Constants.kingCastlingTo["long"][player]));
            Assert.AreEqual(castlingMoves.Count, 1);
        }
    }

    [Test]
    public void TestCastlingMove_WhenKingInCheck()
    {
        foreach (Player player in Constants.Players)
        {
            Piece[,] boardState = BoardTestFactory.CreateCastlingScenario(player);
            Vector2Int kingCoord = player == Player.White ? new Vector2Int(4, 0) : new Vector2Int(4, 7);
            Piece king = boardState[kingCoord.x, kingCoord.y];

            Piece opponentAttacker = BoardTestFactory.CreatePiece(PieceType.Rook, BoardTestFactory.GetOpponent(player));
            Vector2Int attackerPosition = player == Player.White ? new Vector2Int(4, 1) : new Vector2Int(4, 6);
            boardState[attackerPosition.x, attackerPosition.y] = opponentAttacker;
            List<Vector2Int> castlingMoves = _chessLogic.GetCastlingMoves(king, boardState, player);
            Assert.AreEqual(0, castlingMoves.Count);
        }
    }

    [Test]
    public void TestEnPassentMove_WhenAvailable()
    {
        foreach (Player player in Constants.Players)
        {
            Piece[,] board = BoardTestFactory.CreateEmptyBoard();
            int row = player == Player.White ? 4 : 3;

            Vector2Int pawnPos = new(4, row);
            board[pawnPos.x, pawnPos.y] = BoardTestFactory.CreatePiece(PieceType.Pawn, player);

            // Vector2Int opponentPawnPos = new (3, row);
            // board[opponentPawnPos.x, opponentPawnPos.y] = BoardTestFactory.CreatePiece(PieceType.Pawn, BoardTestFactory.GetOpponent(player));

            Vector2Int enPassantTargetPos = new(3, row + Constants.Step[player]);
            Vector2Int? enPassantMove = _chessLogic.GetEnPassantMove(pawnPos, enPassantTargetPos, board, player);

            Assert.IsNotNull(enPassantMove);
            Assert.AreEqual(enPassantTargetPos, enPassantMove.Value);
        }
    }

    [Test]
    public void TestEnPassentMove_WhenNotAvailable()
    {
        foreach (Player player in Constants.Players)
        {
            Piece[,] board = BoardTestFactory.CreateEmptyBoard();
            int row = player == Player.White ? 4 : 3;

            Vector2Int pawnPos = new(4, row);
            board[pawnPos.x, pawnPos.y] = BoardTestFactory.CreatePiece(PieceType.Pawn, player);

            Vector2Int enPassantTargetPos = new(3, row + Constants.Step[player] * 2); // Two steps away - no en passant
            Vector2Int? enPassantMove = _chessLogic.GetEnPassantMove(pawnPos, enPassantTargetPos, board, player);

            Assert.IsNull(enPassantMove);
        }
    }

    [Test]
    public void TestCheckMate_WhenKingNotCheck()
    {
        Piece[,] boardState = BoardTestFactory.CreateEmptyBoard();
        Piece enemyKing = BoardTestFactory.CreatePiece(PieceType.King, Player.Black);
        Piece rook1 = BoardTestFactory.CreatePiece(PieceType.Rook, Player.White);
        Piece rook2 = BoardTestFactory.CreatePiece(PieceType.Rook, Player.White);
        boardState[0, 0] = enemyKing;
        boardState[1, 7] = rook1;
        boardState[7, 1] = rook2;

        bool isCheckMate = _chessLogic.IsInCheckmate(boardState, Player.Black, null);
        bool isInCheck = _chessLogic.IsTileAttacked(new(0, 0), boardState, Player.Black);
        Assert.IsTrue(isCheckMate);
        Assert.IsFalse(isInCheck);
    }

    [Test]
    public void TestCheckMate_WhenKingInCheck()
    {
        Piece[,] boardState = BoardTestFactory.CreateEmptyBoard();
        Piece enemyKing = BoardTestFactory.CreatePiece(PieceType.King, Player.Black);
        Piece queen = BoardTestFactory.CreatePiece(PieceType.Queen, Player.White);
        Piece rook = BoardTestFactory.CreatePiece(PieceType.Rook, Player.White);
        boardState[0, 0] = enemyKing;
        boardState[1, 1] = queen;
        boardState[7, 1] = rook;

        bool isCheckMate = _chessLogic.IsInCheckmate(boardState, Player.Black, null);
        bool isInCheck = _chessLogic.IsTileAttacked(new(0, 0), boardState, Player.Black);
        Assert.IsTrue(isCheckMate);
        Assert.IsTrue(isInCheck);
    }
}
