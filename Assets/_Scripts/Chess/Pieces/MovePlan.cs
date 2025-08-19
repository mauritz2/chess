using UnityEngine;
public class MovePlan
{
    public MoveKind Kind { get; set; }
    public Vector2Int From { get; set; }
    public Vector2Int To { get; set; }
    // Optional extras for special moves below - only set when relevant
    public Vector2Int? CaptureAt { get; set; }
    public Vector2Int? RookFrom { get; set; }
    public Vector2Int? RookTo { get; set; }
    public GameObject? PromotionPiece { get; set; }
}