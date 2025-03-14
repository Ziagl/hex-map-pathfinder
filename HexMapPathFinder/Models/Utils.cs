using com.hexagonsimulations.HexMapBase.Models;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HexMapPathFinder.Tests")]

namespace com.hexagonsimulations.HexMapPathfinder.Models;

internal class Utils
{
    // return a subset of given neighbors that are not unpassable
    public static List<HexTile> WalkableNeighbors(List<HexTile> neighbors, List<int> costMap, int columns)
    {
        List<HexTile> walkableNeighbors = new();

        // filter out all not walkable neighbors
        foreach (var neighbor in neighbors)
        {
            var offset = neighbor.Coordinates.ToOffset();
            int cost = costMap[offset.y * columns + offset.x];
            if (cost > 0)
            {
                walkableNeighbors.Add(neighbor);
            }
        }

        return walkableNeighbors;
    }

    // randomly shuffle a list
    public static List<T> Shuffle<T>(List<T> list)
    {
        Random rng = new Random();
        return list.OrderBy(x => rng.Next()).ToList();
    }
}
