using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Board board;
    [SerializeField] private PieceSetup _pieceSetup;
    [SerializeField] private ChessLogic _chessLogic;
    [SerializeField] private MoveExecutor _moveExecutor;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _pawnPromoteScreen;

    private readonly SelectionManager selectionManager = new();
    private Vector2Int? _enPassantTargetPos = null;
    private MovePlanner _movePlanner;

    public Player ActivePlayer { get; private set; } = Player.White;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _movePlanner = new MovePlanner(_chessLogic);
    }

    void Start()
    {
        board.GenerateGrid();
        _pieceSetup.SetupPieces(board);
        ActivePlayer = Player.White;
    }

    public void OnTileClicked(Tile tile)
    {
        // Selects a tile if none is selected, or tries to move the piece if one is already selected
        if (!selectionManager.HasSelection && tile.Piece != null && tile.Piece.GetComponent<Piece>().Player == ActivePlayer)
        {
            selectionManager.Select(tile);
            ShowValidMoves(tile);
        }
        else
        {
            bool clickedOwnPiece = tile.Piece != null && tile.Piece.GetComponent<Piece>().Player == ActivePlayer;
            if (clickedOwnPiece)
            {
                // User didn't try to move, they wanted to select a new piece
                selectionManager.Deselect();
                board.RemoveAllHighlights();
                OnTileClicked(tile);
                return;
            }

            TryMove(tile);
        }
    }

    private void TryMove(Tile toTile)
    {
        if (toTile == null || !selectionManager.HasSelection)
        {
            Debug.LogWarning("No tile selected or no piece to move.");
            return;
        }
        // 1) Gather inputs
        Tile fromTile = selectionManager.SelectedTile;
        Vector2Int from = board.GetTileCoordinates(fromTile);
        Vector2Int to = board.GetTileCoordinates(toTile);
        Piece[,] boardState = board.GetState();
        Piece movedPiece = selectionManager.SelectedPiece;

        // 2) Plan
        MovePlan plan = _movePlanner.Plan(from, to, boardState, ActivePlayer, _enPassantTargetPos);

        // 3) Early exit if invalid move
        if (plan.Kind == MoveKind.Invalid)
        {
            Debug.LogWarning("Invalid move attempted.");
            selectionManager.Deselect();
            board.RemoveAllHighlights();
            return;
        }

        // 4) Promotion requires UI input for what piece to promote to
        if (plan.Kind == MoveKind.Promotion)
        {
            ShowPawnPromotionScreen(chosen =>
            {
                plan.PromotionPiece = _pieceSetup.GetPiecePrefab(chosen, ActivePlayer);
                _moveExecutor.Execute(plan);
                selectionManager.SelectedPiece.hasMoved = true;
                selectionManager.Deselect();
                board.RemoveAllHighlights();
                ActivePlayer = GetOpponent(ActivePlayer);
            });
            return;
        }

        // 5) Execute move
        _moveExecutor.Execute(plan);

        // 6) Finalize turn and pass to next player if game didn't end
        UpdateEnPassantTarget(from, to, movedPiece);
        movedPiece.hasMoved = true;
        board.RemoveAllHighlights(includeCheckHighlight: true);
        SetCheckHighlight();
        CheckForEndOfGame(GetOpponent(ActivePlayer));
        selectionManager.Deselect();
        ActivePlayer = GetOpponent(ActivePlayer);
    }

    private void SetCheckHighlight()
    {
        foreach (Player kingOwner in Constants.Players)
        {
            Vector2Int kingCoord = board.GetCoordsForKing(kingOwner);

            if (_chessLogic.IsTileAttacked(kingCoord, board.GetState(), kingOwner))
            {
                board.ToggleCheckHighlight(kingCoord, true);
            }
            else
            {
                board.ToggleCheckHighlight(kingCoord, false);
            }
        }
    }


    private void UpdateEnPassantTarget(Vector2Int from, Vector2Int to, Piece movedPiece)
    {
        _enPassantTargetPos = null; // default = no en passant

        if (movedPiece.Type == PieceType.Pawn && Math.Abs(to.y - from.y) == 2)
        {
            // the square the pawn “passed over”
            _enPassantTargetPos = new Vector2Int(to.x, (from.y + to.y) / 2);
        }
    }


    private static Player GetOpponent(Player player)
    {
        return player == Player.White ? Player.Black : Player.White;
    }


    private void ShowPawnPromotionScreen(Action<PieceType> onPromote)
    {
        _pawnPromoteScreen.SetActive(true);
        StartCoroutine(WaitForPawnPromotion());

        IEnumerator WaitForPawnPromotion()
        {
            PieceType selectedPiece = PieceType.None;

            _pawnPromoteScreen.GetComponent<PawnPromoteUI>().OnPieceSelected = pieceType =>
            {
                selectedPiece = pieceType;
            };

            while (selectedPiece == PieceType.None)
            {
                yield return null;
            }

            _pawnPromoteScreen.SetActive(false);
            onPromote?.Invoke(selectedPiece);
        }
    }

    public void CheckForEndOfGame(Player potentialLoser)
    {
        Piece[,] boardState = board.GetState();
        Vector2Int kingCoord = board.GetCoordsForKing(potentialLoser);
        List<Player> winners = new();

        bool isCheckMate = _chessLogic.IsInCheckmate(boardState, potentialLoser, _enPassantTargetPos);
        bool isInCheck = _chessLogic.IsTileAttacked(kingCoord, boardState, potentialLoser);

        if (isCheckMate && !isInCheck)
        {
            winners.Add(potentialLoser);
            winners.Add(GetOpponent(potentialLoser));
            EndGame(winners);
        }
        else if (isCheckMate && isInCheck)
        {
            winners.Add(GetOpponent(potentialLoser));
            EndGame(winners);
        }

        if (board.OnlyKingsRemain(boardState))
        {
            winners.Add(potentialLoser);
            winners.Add(GetOpponent(potentialLoser));
            EndGame(winners);
        }
    }


    private void EndGame(List<Player> winners)
    {
        _gameOverScreen.SetActive(true);

        if (winners.Count > 1)
        {
            FindObjectOfType<GameUI>().SetGameOverText("Game draw!");
        }
        else if (winners.Count == 1)
        {
            Player winner = winners[0];
            FindObjectOfType<GameUI>().SetGameOverText($"Checkmate! {winner} wins!");
        }
    }

    public static void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    private void ShowValidMoves(Tile fromTile)
    {
        Vector2Int from = board.GetTileCoordinates(fromTile);
        List<Vector2Int> moves = _chessLogic.GetValidMoves(from, board.GetState(), ActivePlayer, _enPassantTargetPos);

        foreach (Vector2Int move in moves)
        {
            Tile toTile = board.GetTileAt(move);
            if (toTile.Piece == null)
            {
                board.ToggleMoveHighlight(move, moveHighlight: true);
            }
            else if (toTile.Piece.GetComponent<Piece>().Player != ActivePlayer)
            {
                board.ToggleMoveHighlight(move, attackHighlight: true);
            }
        }
    }
}