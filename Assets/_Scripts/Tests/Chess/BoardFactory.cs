using UnityEngine;

public static class BoardTestFactory
{
    public static Piece[,] CreateEmptyBoard(int width = 8, int height = 8)
    {
        return new Piece[width, height];
    }

    public static Piece[,] CreateBoardWithPiece(Vector2Int position, Piece piece)
    {
        var board = CreateEmptyBoard();
        board[position.x, position.y] = piece;
        return board;
    }


    public static Piece CreatePiece(PieceType type, Player player, bool hasMoved = false)
    {
        Piece piece = type switch
        {
            // Need to instantiate specific piecetype since you can't instantiate abstract class Piece 
            PieceType.Pawn => new GameObject($"{player}_{type}").AddComponent<Pawn>(),
            PieceType.Rook => new GameObject($"{player}_{type}").AddComponent<Rook>(),
            PieceType.Knight => new GameObject($"{player}_{type}").AddComponent<Knight>(),
            PieceType.Queen => new GameObject($"{player}_{type}").AddComponent<Queen>(),
            PieceType.King => new GameObject($"{player}_{type}").AddComponent<King>(),
            PieceType.Bishop => new GameObject($"{player}_{type}").AddComponent<Bishop>(),
            _ => throw new System.ArgumentException($"Unknown piece type: {type}")
        };
        piece.Type = type;
        piece.Player = player;
        piece.hasMoved = hasMoved;
        return piece;
    }

    public static Piece[,] CreateCastlingScenario(Player player, bool kingHasMoved = false, bool rookHasMoved = false)
    {
        var board = CreateEmptyBoard();

        int row = player == Player.White ? 0 : 7;

        // King
        board[4, row] = CreatePiece(PieceType.King, player, kingHasMoved);

        // Rooks
        board[0, row] = CreatePiece(PieceType.Rook, player, rookHasMoved); // Queenside
        board[7, row] = CreatePiece(PieceType.Rook, player, rookHasMoved); // Kingside

        return board;
    }

    public static Piece[,] CreateEnPassantScenario(Player player)
    {
        var board = CreateEmptyBoard();

        // hero pawn
        int row = player == Player.White ? 4 : 3;
        board[4, row] = CreatePiece(PieceType.Pawn, player);

        // opponent pawn
        board[3, row] = CreatePiece(PieceType.Rook, player);

        return board;
    }

    public static Player GetOpponent(Player player)
    {
        return player == Player.White ? Player.Black : Player.White;
    }
}
