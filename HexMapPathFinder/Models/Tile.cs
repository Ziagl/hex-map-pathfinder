using com.hexagonsimulations.Geometry.Hex;

namespace hex_map_pathfinder.Models;

internal class Tile : HexTile
{
    public int MovementCost = 0;
    public int EstimatedMovementCost = 0;
    public int Sum = 0;
}
