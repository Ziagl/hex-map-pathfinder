using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapPathfinder.Models;

namespace com.hexagonsimulations.HexMapPathFinder.Tests;

[TestClass]
public sealed class UtilsTests
{
    [TestMethod]
    public void Shuffle()
    {
        List<int> list = new() { 1, 2, 3, 4, 5 };
        list = Utils.Shuffle(list);
        Assert.AreNotEqual(new List<int> { 1, 2, 3, 4, 5 }, list);
    }

    [TestMethod]
    public void WalkableNeighbors()
    {
        List<int> costMap = new() { 2, 1, 1, 2, 1, 0, 0, 1, 2, 0, 0, 1, 1, 1, 1, 1 };
        var grid = HexGrid.InitializeGrid<HexTile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(1, 1, -2) };
        var neighbors = tile.Neighbors(grid, 4, 4);
        var walkableNeighbors = Utils.WalkableNeighbors(neighbors, costMap, 4);
        Assert.HasCount(3, walkableNeighbors);
    }
}