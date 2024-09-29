using com.hexagonsimulations.Geometry.Hex;
using hex_map_pathfinder.Models;
using Xunit;

namespace HexMapPathFinder.Tests;

public class PathFinderTests
{
    [Fact]
    public void TestComputePath()
    {
        List<int> costMap = new() { 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var path = pathFinder.ComputePath(new CubeCoordinates(0, 0, 0), new CubeCoordinates(2, 3, -5), 0);
        Assert.Equal(7, path.Count);
    }
}
