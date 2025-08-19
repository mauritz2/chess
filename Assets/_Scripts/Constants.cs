using System.Collections.Generic;
using UnityEngine;
public static class Constants
{
    public const int BoardWidth = 8;
    public const int BoardHeight = 8;
    public static readonly List<Player> Players = new() { Player.White, Player.Black };
    public static readonly Dictionary<Player, int> Step = new()
    {
        { Player.White, 1},
        { Player.Black, -1}
    };
    public static readonly Dictionary<string, Dictionary<Player, Vector2Int>> rookCastlingFrom = new()
    {
        ["long"] = new()
            {
                { Player.White, new(0, 0) },
                { Player.Black, new(0, 7) }
            },
        ["short"] = new()
            {
                { Player.White, new(7, 0) },
                { Player.Black, new(7, 7) }
            }
    };
    public static readonly Dictionary<string, Dictionary<Player, Vector2Int>> rookCastlingTo = new()
    {
        ["long"] = new()
            {
                { Player.White, new(2, 0) },
                { Player.Black, new(2, 7) }
            },
        ["short"] = new()
            {
                { Player.White, new(5, 0) },
                { Player.Black, new(5, 7) }
            }
    };

    public static readonly Dictionary<string, Dictionary<Player, Vector2Int>> kingCastlingTo = new()
    {
        ["long"] = new()
        {
            { Player.White, new(1, 0) },
            { Player.Black, new(1, 7) }
        },
        ["short"] = new()
        {
            { Player.White, new(6, 0) },
            { Player.Black, new(6, 7) }
        }
    };
}