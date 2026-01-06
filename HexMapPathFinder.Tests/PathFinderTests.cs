using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapPathfinder;
using com.hexagonsimulations.HexMapPathfinder.Models;

namespace com.hexagonsimulations.HexMapPathFinder.Tests;

[TestClass]
public sealed class PathFinderTests
{
    [TestMethod]
    public void GetCostMap()
    {  
        List<int> costMap = new() { 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var retrievedCostMap = pathFinder.GetCostMap(0);
        CollectionAssert.AreEqual(costMap, retrievedCostMap);
    }

    [TestMethod]
    public void ComputePath()
    {
        List<int> costMap = new() { 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var path = pathFinder.ComputePath(new CubeCoordinates(0, 0, 0), new CubeCoordinates(2, 3, -5), 0);
        Assert.HasCount(7, path);
    }

    [TestMethod]
    public void ComputePathOffsetCoordinates()
    {
        List<int> costMap = new() { 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var path = pathFinder.ComputePathOffsetCoordinates(new OffsetCoordinates(0, 0), new OffsetCoordinates(3, 3), 0);
        Assert.HasCount(7, path);
    }

    [TestMethod]
    public void ComputePathWithObstacles()
    {
        List<int> costMap = new() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var blockingObstacles = new List<CubeCoordinates>() { new CubeCoordinates(1, 0, -1) };
        // test for different possible paths
        for (int i = 0; i < 10; ++i)
        {
            var path = pathFinder.ComputePath(new CubeCoordinates(0, 0, 0), new CubeCoordinates(1, 1, -2), 0, blockingObstacles);
            Assert.AreNotEqual(path[1], new CubeCoordinates(1, 0, -1), "Obstacle is part of path.");
            Assert.HasCount(3, path);
        }
    }

    [TestMethod]
    public void FindComplexPath()
    {
        List<int> costMap = new() { 1, 1, 2, 3, 1, 2, 1, 3, 2, 4, 8, 1, 3, 1, 2, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var path = pathFinder.ComputePathOffsetCoordinates(new OffsetCoordinates(0, 0), new OffsetCoordinates(3, 3), 0);
        Assert.HasCount(6, path);
    }

    [TestMethod]
    public void NoPossiblePath()
    {
        List<int> costMap = new() { 1, 1, 2, 1, 1, 2, 3, 3, 2, 2, 8, 0, 1, 1, 0, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var path = pathFinder.ComputePathOffsetCoordinates(new OffsetCoordinates(0, 0), new OffsetCoordinates(3, 3), 0);
        Assert.IsEmpty(path);
    }

    [TestMethod]
    public void ReachableTiles()
    {
        List<int> costMap = new() { 1, 1, 2, 3, 1, 2, 1, 3, 2, 4, 8, 1, 3, 1, 2, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(0, 0, 0), 2, 0);
        Assert.HasCount(6, reachableTiles);
    }

    [TestMethod]
    public void ReachableTilesWithObstacles()
    {
        List<int> costMap = new() { 1, 1, 2, 3, 1, 2, 1, 3, 2, 4, 8, 1, 3, 1, 2, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var blockingObstacles = new List<CubeCoordinates>() { new CubeCoordinates(1, 0, -1) };
        var nonBlockingObstacles = new List<CubeCoordinates>() { new CubeCoordinates(0, 0, 0), new CubeCoordinates(0, 1, -1) };
        var reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(0, 0, 0), 2, 0, nonBlockingObstacles, blockingObstacles);
        Assert.HasCount(3, reachableTiles);
    }

    [TestMethod]
    public void ReachableTiles2()
    {
        List<int> costMap = new() { 8, 8, 8, 3, 8, 1, 1, 4, 8, 8, 1, 1, 3, 9, 1, 2 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(1, 1, -2), 2, 0);
        Assert.HasCount(11, reachableTiles);
    }

    [TestMethod]
    public void AttackableTiles()
    {
        List<int> costMap = new() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var attackableTiles = pathFinder.AttackableTiles(new CubeCoordinates(0, 0, 0), 2, 2, 0, new List<CubeCoordinates>(), new List<CubeCoordinates>()
        {
            new CubeCoordinates(1, 0, -1),
            new CubeCoordinates(2, 1, -3),
            new CubeCoordinates(2, 2, -4),
            new CubeCoordinates(-1, 2, -1)
        });
        Assert.HasCount(3, attackableTiles);
        attackableTiles = pathFinder.AttackableTiles(new CubeCoordinates(0, 0, 0), 0, 2, 0, new List<CubeCoordinates>(), new List<CubeCoordinates>()
        {
            new CubeCoordinates(1, 0, -1),
            new CubeCoordinates(2, 1, -3),
            new CubeCoordinates(2, 2, -4),
            new CubeCoordinates(-1, 2, -1)
        });
        Assert.IsEmpty(attackableTiles, $"With maxCost there should not be any attackable tiles, but found {attackableTiles.Count}");
        // test complex case which leads to errors
        var blockingObstacles = new List<CubeCoordinates>() { new CubeCoordinates(1, 0, -1), new CubeCoordinates(2, 1, -3), 
                                                              new CubeCoordinates(0, 2, -2), new CubeCoordinates(1, 2, -3) };
        var nonBlockingObstacles = new List<CubeCoordinates>() { };
        attackableTiles = pathFinder.AttackableTiles(new CubeCoordinates(1, 1, -2), 2, 1, 0, nonBlockingObstacles, blockingObstacles);
        Assert.HasCount(4, attackableTiles);
    }

    [TestMethod]
    public void WrongLayer()
    {
        List<int> costMap = new() { 8, 8, 8, 3, 8, 1, 1, 4, 8, 8, 1, 1, 3, 9, 1, 2 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(1, 1, -2), 2, 1);
        Assert.IsEmpty(reachableTiles);
    }

    [TestMethod]
    public void NeighborTiles()
    {
        var pathFinder = new PathFinder(new List<List<int>>(), 46, 74);
        var baseCoordinate = new CubeCoordinates(0, 0, 0);
        var neighborCoordinates = pathFinder.NeighborTiles(baseCoordinate);
        Assert.HasCount(2, neighborCoordinates);
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(1, 0, -1));
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(0, 1, -1));
        baseCoordinate = new CubeCoordinates(1, 1, -2);
        neighborCoordinates = pathFinder.NeighborTiles(baseCoordinate);
        Assert.HasCount(6, neighborCoordinates);
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(1, 0, -1));
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(0, 1, -1));
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(2, 0, -2));
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(2, 1, -3));
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(1, 2, -3));
        CollectionAssert.Contains(neighborCoordinates, new CubeCoordinates(0, 2, -2));
    }

    [TestMethod]
    public void BugDistanceGreater2()
    {
        List<int> costMap = Enumerable.Repeat(1, 144).ToList();
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 12, 12);
        var baseCoordinate = new OffsetCoordinates(5, 5).ToCubic();
        var reachableTiles = pathFinder.ReachableTiles(baseCoordinate, 3, 0);
        Assert.HasCount(36, reachableTiles);
    }

    [TestMethod]
    public void CreateWeightedPath()
    {
        List<int> costs = new() { 1, 1, 1, 1, 2, 1, 1, 1, 3, 1, 1, 1, 4, 1, 1, 1 };
        var pathFinder = new PathFinder(new List<List<int>>() { costs }, 4, 4);
        List<CubeCoordinates> path = new() { new CubeCoordinates(0, 0, 0),
                                             new CubeCoordinates(0, 1, -1),
                                             new CubeCoordinates(-1, 2, -1),
                                             new CubeCoordinates(-1, 3, -2) };
        var weightedPath = pathFinder.CreateWeightedPath(path, 0);
        Assert.HasCount(4, weightedPath);
        Assert.AreEqual(3, weightedPath[2].Cost);
    }

    [TestMethod]
    public void RiverReachableTiles()
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
        Assert.HasCount(14, reachableTiles);
        reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(2, 0, 0), 4, 0);
        Assert.HasCount(16, reachableTiles);
        reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(4, 0, 0), 4, 0);
        Assert.HasCount(9, reachableTiles);
        reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(3, 0, 0), 4, 0);
        Assert.HasCount(10, reachableTiles);
    }
}
