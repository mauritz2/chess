using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

public class MovePlannerTest
{
    private MovePlanner _movePlanner;
    private ChessLogic _chessLogic;

    public MovePlannerTest()
    {
        _chessLogic = new ChessLogic();
        _movePlanner = new MovePlanner(_chessLogic);
    }

    [Test]
    public void TestCastlingMove_WhenNoPiecesMoved()
    {
        foreach (Player player in Constants.Players)
        {
            Piece[,] boardState = BoardTestFactory.CreateCastlingScenario(player);
            Vector2Int kingCoord = player == Player.White ? new Vector2Int(4, 0) : new Vector2Int(4, 7);
            MovePlan movePlan = _movePlanner.Plan(
                kingCoord,
                Constants.kingCastlingTo["short"][player],
                boardState,
                player,
                null
            );
            Assert.AreEqual(MoveKind.Castle, movePlan.Kind);

        }
    }

    [Test]
    public void TestCastlingMove_WhenKingHasMoved()
    {
        foreach (Player player in Constants.Players)
        {
            Piece[,] boardState = BoardTestFactory.CreateCastlingScenario(player, true, false);
            Vector2Int kingCoord = player == Player.White ? new Vector2Int(4, 0) : new Vector2Int(4, 7);
            MovePlan movePlan = _movePlanner.Plan(
                kingCoord,
                Constants.kingCastlingTo["short"][player],
                boardState,
                player,
                null
            );
            Assert.AreEqual(MoveKind.Invalid, movePlan.Kind);
        }
    }

    [Test]
    public void TestCastlingMove_WhenRooksHaveMoved()
    {
        foreach (Player player in Constants.Players)
        {
            Piece[,] boardState = BoardTestFactory.CreateCastlingScenario(player, false, true);
            Vector2Int kingCoord = player == Player.White ? new Vector2Int(4, 0) : new Vector2Int(4, 7);
            MovePlan movePlan = _movePlanner.Plan(
                kingCoord,
                Constants.kingCastlingTo["short"][player],
                boardState,
                player,
                null
            );
            Assert.AreEqual(MoveKind.Invalid, movePlan.Kind);
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
            MovePlan movePlan = _movePlanner.Plan(
                kingCoord,
                Constants.kingCastlingTo["long"][player],
                boardState,
                player,
                null
            );
            Assert.AreEqual(MoveKind.Castle, movePlan.Kind);
        }
    }

    [Test]
    public void TestEnPassent_Moves_WhenTargetPawnExists()
    {

        Dictionary<Player, Player> opponents = new()
        {
            { Player.White, Player.Black },
            { Player.Black, Player.White }
        };

        foreach (Player player in Constants.Players)
        {
            int[] directions = { 1, -1 };
            foreach (int direction in directions)
            {
                Vector2Int pawnPos = new(4, player == Player.White ? 4 : 3);
                Piece pawn = BoardTestFactory.CreatePiece(PieceType.Pawn, player);
                Piece secondPawn = BoardTestFactory.CreatePiece(PieceType.Pawn, opponents[player]);

                Piece[,] boardState = BoardTestFactory.CreateBoardWithPiece(pawnPos, pawn);
                boardState[pawnPos.x + direction, pawnPos.y] = secondPawn;

                Vector2Int enPassantTargetPos = new(pawnPos.x + direction, pawnPos.y + Constants.Step[player]);

                MovePlan movePlan = _movePlanner.Plan(
                    pawnPos,
                    enPassantTargetPos,
                    boardState,
                    player,
                    enPassantTargetPos
                );
                Assert.AreEqual(MoveKind.EnPassant, movePlan.Kind);
                Assert.AreEqual(new Vector2Int(pawnPos.x + direction, pawnPos.y), movePlan.CaptureAt);
            }
        }
    }

    [Test]
    public void TestCapture_WhenAvailable()
    {
        Vector2Int pos = new(1, 1);
        Piece queen = BoardTestFactory.CreatePiece(PieceType.Queen, Player.White);
        Vector2Int enemyPos = new(0, 0);
        Piece pawn = BoardTestFactory.CreatePiece(PieceType.Pawn, Player.Black);

        Piece[,] boardState = BoardTestFactory.CreateBoardWithPiece(pos, queen);
        boardState[enemyPos.x, enemyPos.y] = pawn;

        MovePlan movePlan = _movePlanner.Plan(
            pos,
            enemyPos,
            boardState,
            Player.White,
            null
        );
        Assert.AreEqual(MoveKind.Capture, movePlan.Kind);
        Assert.AreEqual(enemyPos, movePlan.CaptureAt);
    }

    [Test]
    public void TestQuietMove_WhenNoCaptureAvailable()
    {
        Vector2Int from = new(1, 1);
        Vector2Int to = new(7, 7);
        Piece queen = BoardTestFactory.CreatePiece(PieceType.Queen, Player.White);
        Piece[,] boardState = BoardTestFactory.CreateBoardWithPiece(from, queen);

        MovePlan movePlan = _movePlanner.Plan(
            from,
            to,
            boardState,
            Player.White,
            null
        );
        Assert.AreEqual(MoveKind.Quiet, movePlan.Kind);
        Assert.AreEqual(null, movePlan.CaptureAt);
    }
}