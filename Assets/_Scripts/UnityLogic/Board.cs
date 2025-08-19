using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Board : MonoBehaviour
{
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;

    private Dictionary<Vector2Int, Tile> _tiles;

    public int Width, Height;

    public void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2Int, Tile>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);

                _tiles[new Vector2Int(x, y)] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3((float)Width / 2 - 0.5f, (float)Height / 2 - 0.5f, -10);
    }

    public Tile GetTileAt(Vector2Int coordinates)
    {
        if (_tiles.TryGetValue(coordinates, out var tile))
        {
            return tile;
        }

        return null;
    }

    public Vector2Int GetCoordsForKing(Player player)
    {
        foreach (var tile in _tiles.Values)
        {
            if (tile.Piece != null && tile.Piece.GetComponent<Piece>().Player == player &&
                tile.Piece.GetComponent<Piece>().Type == PieceType.King)
            {
                return GetTileCoordinates(tile);
            }
        }
        throw new InvalidOperationException($"No king found for player {player}");
    }

    public Vector2Int GetTileCoordinates(Tile tile)
    {
        List<string> _coordsString = tile.name.Split(" ").Skip(1).ToList();
        return new Vector2Int(int.Parse(_coordsString[0]), int.Parse(_coordsString[1]));
    }
    

        public List<Vector2Int> GetAllTilePositions()
    {
        return _tiles.Keys.ToList();
    }

    public Piece[,] GetState()
    {
        Piece[,] pieces = new Piece[Width, Height];
        foreach (var tile in _tiles.Values)
        {
            if (tile.Piece != null)
            {
                Vector2Int coords = GetTileCoordinates(tile);
                pieces[coords.x, coords.y] = tile.Piece.GetComponent<Piece>();
            }
        }
        return pieces;
    }


    public bool OnlyKingsRemain(Piece[,] board)
    {
        var pieces = board.Cast<Piece>().Where(p => p != null).ToList();
        return pieces.Count == 2
            && pieces.All(p => p.Type == PieceType.King)
            && pieces.Select(p => p.Player).Distinct().Count() == 2;
    }

    public void ToggleMoveHighlight(Vector2Int pos, bool moveHighlight = false, bool attackHighlight = false)
    {
        Tile tile = GetTileAt(pos);
        tile.MoveHighlight.SetActive(moveHighlight);
        tile.AttackHighlight.SetActive(attackHighlight);
    }

    public void ToggleCheckHighlight(Vector2Int pos, bool checkHighlight = false)
    {
        Tile tile = GetTileAt(pos);
        tile.CheckHighlight.SetActive(checkHighlight);
    }


    public void RemoveAllHighlights(bool includeCheckHighlight = false)
    {
        foreach (var tile in _tiles.Values)
        {
            ToggleMoveHighlight(GetTileCoordinates(tile), false, false);

            if (includeCheckHighlight)
            {
                ToggleCheckHighlight(GetTileCoordinates(tile), false);
            }
        }
    }

}