using com.hexagonsimulations.Geometry.Hex;
using hex_map_pathfinder.Models;

public class PathFinder
{
    private readonly int MAXLOOPS = 10000;
    private MapData _map = new();

    public PathFinder(List<List<int>> costMap, int rows, int columns)
    {
        _map.Map = costMap;
        _map.Rows = rows;
        _map.Columns = columns;
    }

    public List<CubeCoordinates> ComputePath(CubeCoordinates start, CubeCoordinates end, int layerIndex)
    {
        if (layerIndex < 0 || layerIndex >= _map.Map.Count)
        {
            return new List<CubeCoordinates>();
        }
        // create empty grid
        List<Tile> grid = HexGrid.InitializeGrid<Tile>(_map.Rows, _map.Columns);
        List<CubeCoordinates> path = new();
        List<Tile> openList = new();
        List<Tile> closedList = new();
        openList.Add(new Tile()
        {
            Coordinates = start,
            MovementCost = 0,
            EstimatedMovementCost = CubeCoordinates.Distance(start, end),
        });
        // compute AStar algorithm
        bool pathFound = false;
        int loopMax = this.MAXLOOPS;
        do
        {
            // remove tile from open list
            var tile = openList.First();
            openList.RemoveAt(0);
            // and add it to closed list
            closedList.Add(tile);
            // if tile is end, break
            if (tile.Coordinates == end)
            {
                pathFound = true;
                break;
            }
            // if start tile is not passable, break
            if (_map.Map[layerIndex][start.ToOffset().x * _map.Rows + start.ToOffset().y] == 0)
            {
                pathFound = false;
                break;
            }
            // get walkable neighbors
            var neighbors = tile.Neighbors(grid.Cast<HexTile>().ToList(), _map.Rows, _map.Columns);
            var walkableNeighbors = Utils.WalkableNeighbors(neighbors, _map.Map[layerIndex], _map.Columns).Cast<Tile>().ToList();
            // for every walkable neighbor
            foreach (var walkableNeighbor in walkableNeighbors)
            {
                // if neighbor is in closed list, skip it
                if (closedList.Contains(walkableNeighbor))
                {
                    continue;
                }
                // if neighbor is not in open list, add it
                if (!openList.Contains(walkableNeighbor))
                {
                    var offset = walkableNeighbor.Coordinates.ToOffset();
                    int tileMovementCost = _map.Map[layerIndex][offset.y * _map.Columns + offset.x];
                    walkableNeighbor.MovementCost = tile.MovementCost + tileMovementCost;
                    walkableNeighbor.EstimatedMovementCost = CubeCoordinates.Distance(walkableNeighbor.Coordinates, end);
                    walkableNeighbor.Sum = walkableNeighbor.MovementCost + walkableNeighbor.EstimatedMovementCost;
                    openList.Insert(0, walkableNeighbor);
                }
                // if neighbor is in open list and has a lower cost, update it
                else
                {
                    var existingTile = openList.Find(t => t.Coordinates == walkableNeighbor.Coordinates);
                    var offset = walkableNeighbor.Coordinates.ToOffset();
                    int tileMovementCost = _map.Map[layerIndex][offset.y * _map.Columns + offset.x];
                    if (existingTile is not null && existingTile.MovementCost > tile.MovementCost + tileMovementCost)
                    {
                        existingTile.MovementCost = tile.MovementCost + tileMovementCost;
                        existingTile.EstimatedMovementCost = CubeCoordinates.Distance(existingTile.Coordinates, end);
                        existingTile.Sum = existingTile.MovementCost + existingTile.EstimatedMovementCost;
                    }
                }
            }
            --loopMax;
        } while (openList.Count > 0 && pathFound == false && loopMax > 0);
        // reconstruct path
        if (pathFound)
        {
            var current = closedList.Last();
            closedList.RemoveAt(closedList.Count - 1);
            loopMax = this.MAXLOOPS;
            while (current is not null && loopMax > 0)
            {
                // add end coordinates
                path.Add(current.Coordinates);
                // if start is reachend -> end loop
                if (current.Coordinates == start)
                {
                    current = null;
                }
                else
                {
                    var neighbors = current.Neighbors(grid.Cast<HexTile>().ToList(), _map.Rows, _map.Columns).Cast<Tile>().ToList();
                    neighbors = Utils.Shuffle(neighbors);
                    foreach (var neighbor in neighbors)
                    {
                        var nextTile = closedList.Find(t => t.Coordinates == neighbor.Coordinates);
                        if (nextTile is not null && neighbor.MovementCost < current.MovementCost)
                        {
                            current = nextTile;
                            break;
                        }
                    }    
                }
                --loopMax;
            }
        }
        path.Reverse();
        return path;
    }
}