using com.hexagonsimulations.Geometry.Hex;
using hex_map_pathfinder.Models;

namespace com.hexagonsimulations.HexMapPathFinder.Tests;

[TestClass]
public sealed class PathFinderTests
{
    [TestMethod]
    public void TestComputePath()
    {
        List<int> costMap = new() { 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var path = pathFinder.ComputePath(new CubeCoordinates(0, 0, 0), new CubeCoordinates(2, 3, -5), 0);
        Assert.AreEqual(7, path.Count);
    }

    [TestMethod]
    public void TestComputePathOffsetCoordinates()
    {
        List<int> costMap = new() { 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var path = pathFinder.ComputePathOffsetCoordinates(new OffsetCoordinates(0, 0), new OffsetCoordinates(3, 3), 0);
        Assert.AreEqual(7, path.Count);
    }

    [TestMethod]
    public void TestFindComplexPath()
    {
        List<int> costMap = new() { 1, 1, 2, 3, 1, 2, 1, 3, 2, 4, 8, 1, 3, 1, 2, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var path = pathFinder.ComputePathOffsetCoordinates(new OffsetCoordinates(0, 0), new OffsetCoordinates(3, 3), 0);
        Assert.AreEqual(6, path.Count);
    }

    [TestMethod]
    public void TestNoPossiblePath()
    {
        List<int> costMap = new() { 1, 1, 2, 1, 1, 2, 3, 3, 2, 2, 8, 0, 1, 1, 0, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var path = pathFinder.ComputePathOffsetCoordinates(new OffsetCoordinates(0, 0), new OffsetCoordinates(3, 3), 0);
        Assert.AreEqual(0, path.Count);
    }

    [TestMethod]
    public void TestReachableTiles()
    {
        List<int> costMap = new() { 1, 1, 2, 3, 1, 2, 1, 3, 2, 4, 8, 1, 3, 1, 2, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(0, 0, 0), 2, 0);
        Assert.AreEqual(6, reachableTiles.Count);
    }

    [TestMethod]
    public void TestReachableTiles2()
    {
        List<int> costMap = new() { 8, 8, 8, 3, 8, 1, 1, 4, 8, 8, 1, 1, 3, 9, 1, 2 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(1, 1, -2), 2, 0);
        Assert.AreEqual(11, reachableTiles.Count);
    }

    [TestMethod]
    public void TestWrongLayer()
    {
        List<int> costMap = new() { 8, 8, 8, 3, 8, 1, 1, 4, 8, 8, 1, 1, 3, 9, 1, 2 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(1, 1, -2), 2, 1);
        Assert.AreEqual(0, reachableTiles.Count);
    }

    [TestMethod]
    public void TestNeighborTiles()
    {
        var pathFinder = new PathFinder(new List<List<int>>(), 46, 74);
        var baseCoordinate = new CubeCoordinates(0, 0, 0);
        var neighborCoordinates = pathFinder.NeighborTiles(baseCoordinate);
        Assert.AreEqual(2, neighborCoordinates.Count);
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(1, 0, -1));
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(0, 1, -1));
        baseCoordinate = new CubeCoordinates(1, 1, -2);
        neighborCoordinates = pathFinder.NeighborTiles(baseCoordinate);
        Assert.AreEqual(6, neighborCoordinates.Count);
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(1, 0, -1));
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(0, 1, -1));
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(2, 0, -2));
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(2, 1, -3));
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(1, 2, -3));
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(0, 2, -2));
    }

    [TestMethod]
    public void TestBugDistanceGreater2()
    {
        List<int> costMap = Enumerable.Repeat(1, 144).ToList();
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 12, 12);
        var baseCoordinate = new OffsetCoordinates(5, 5).ToCubic();
        var reachableTiles = pathFinder.ReachableTiles(baseCoordinate, 3, 0);
        Assert.AreEqual(36, reachableTiles.Count);
    }

    [TestMethod]
    public void TestCreateWeightedPath()
    {
        List<int> costs = new() { 1, 1, 1, 1, 2, 1, 1, 1, 3, 1, 1, 1, 4, 1, 1, 1 };
        var pathFinder = new PathFinder(new List<List<int>>() { costs }, 4, 4);
        List<CubeCoordinates> path = new() { new CubeCoordinates(0, 0, 0),
                                             new CubeCoordinates(0, 1, -1),
                                             new CubeCoordinates(-1, 2, -1),
                                             new CubeCoordinates(-1, 3, -2) };
        var weightedPath = pathFinder.CreateWeightedPath(path, 0);
        Assert.AreEqual(4, weightedPath.Count);
        Assert.AreEqual(3, weightedPath[2].Cost);
    }

    [TestMethod]
    public void TestRiverReachableTiles()
    {
        List<int> costs = new() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        List<TileProperty> properties = new() {
            TileProperty.NONE, TileProperty.NONE, TileProperty.RIVER, TileProperty.RIVERBANK, TileProperty.NONE,
            TileProperty.NONE, TileProperty.NONE, TileProperty.RIVER, TileProperty.RIVERBANK, TileProperty.NONE,
            TileProperty.NONE, TileProperty.NONE, TileProperty.RIVER, TileProperty.RIVERBANK, TileProperty.NONE,
            TileProperty.NONE, TileProperty.NONE, TileProperty.RIVER, TileProperty.RIVERBANK, TileProperty.NONE,
            TileProperty.NONE, TileProperty.NONE, TileProperty.RIVER, TileProperty.RIVERBANK, TileProperty.NONE
        };
        var pathFinder = new PathFinder(new List<List<int>>() { costs }, new List<List<TileProperty>>() { properties }, 5, 5);
        var reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(0, 0, 0), 4, 0);
        Assert.AreEqual(14, reachableTiles.Count);
        reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(2, 0, 0), 4, 0);
        Assert.AreEqual(16, reachableTiles.Count);
        reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(4, 0, 0), 4, 0);
        Assert.AreEqual(9, reachableTiles.Count);
        reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(3, 0, 0), 4, 0);
        Assert.AreEqual(10, reachableTiles.Count);
    }
}
