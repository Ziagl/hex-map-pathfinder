using com.hexagonsimulations.Geometry.Hex;
using hex_map_pathfinder.Models;
using Xunit;

namespace HexMapPathFinder.Tests;

public class UtilsTests
{
    [Fact]
    public void TestShuffle()
    {
        List<int> list = new() { 1, 2, 3, 4, 5 };
        list = Utils.Shuffle(list);
        Assert.NotEqual(new List<int> { 1, 2, 3, 4, 5 }, list);
    }

    [Fact]
    public void TestWalkableNeighbors()
    {
        List<int> costMap = new() { 2, 1, 1, 2, 1, 0, 0, 1, 2, 0, 0, 1, 1, 1, 1, 1 };
        var grid = HexGrid.InitializeGrid<HexTile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(1, 1, -2) };
        var neighbors = tile.Neighbors(grid, 4, 4);
        var walkableNeighbors = Utils.WalkableNeighbors(neighbors, costMap, 4);
        Assert.Equal(3, walkableNeighbors.Count);
    }
}
