﻿using com.hexagonsimulations.HexMapBase.Models;

namespace com.hexagonsimulations.HexMapPathfinder.Models;

internal class Tile : HexTile
{
    public int MovementCost = 0;
    public int EstimatedMovementCost = 0;
    public int Sum = 0;
}
