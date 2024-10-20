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

    [Fact]
    public void TestComputePathOffsetCoordinates()
    {
        List<int> costMap = new() { 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var path = pathFinder.ComputePathOffsetCoordinates(new OffsetCoordinates(0, 0), new OffsetCoordinates(3, 3), 0);
        Assert.Equal(7, path.Count);
    }

    [Fact]
    public void TestFindComplexPath()
    {
        List<int> costMap = new() { 1, 1, 2, 3, 1, 2, 1, 3, 2, 4, 8, 1, 3, 1, 2, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var path = pathFinder.ComputePathOffsetCoordinates(new OffsetCoordinates(0, 0), new OffsetCoordinates(3, 3), 0);
        Assert.Equal(6, path.Count);
    }

    [Fact]
    public void TestNoPossiblePath()
    {
        List<int> costMap = new() { 1, 1, 2, 1, 1, 2, 3, 3, 2, 2, 8, 0, 1, 1, 0, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var path = pathFinder.ComputePathOffsetCoordinates(new OffsetCoordinates(0, 0), new OffsetCoordinates(3, 3), 0);
        Assert.Empty(path);
    }

    [Fact]
    public void TestReachableTiles()
    {
        List<int> costMap = new() { 1, 1, 2, 3, 1, 2, 1, 3, 2, 4, 8, 1, 3, 1, 2, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(0, 0, 0), 2, 0);
        Assert.Equal(7, reachableTiles.Count);
    }

    [Fact]
    public void TestReachableTiles2()
    {
        List<int> costMap = new() { 8, 8, 8, 3, 8, 1, 1, 4, 8, 8, 1, 1, 3, 9, 1, 2 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(1, 1,-2), 2, 0);
        Assert.Equal(12, reachableTiles.Count);
    }

    [Fact]
    public void TestWrongLayer()
    {
        List<int> costMap = new() { 8, 8, 8, 3, 8, 1, 1, 4, 8, 8, 1, 1, 3, 9, 1, 2 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
        var reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(1, 1, -2), 2, 1);
        Assert.Empty(reachableTiles);
    }

    [Fact]
    public void TestNeighborTiles()
    {
        var pathFinder = new PathFinder(new List<List<int>>(), 46, 74);
        var baseCoordinate = new CubeCoordinates(0, 0, 0);
        var neighborCoordinates = pathFinder.NeighborTiles(baseCoordinate);
        Assert.Equal(2, neighborCoordinates.Count);
        Assert.Contains(new CubeCoordinates(1, 0, -1), neighborCoordinates);
        Assert.Contains(new CubeCoordinates(0, 1, -1), neighborCoordinates);
        baseCoordinate = new CubeCoordinates(1, 1, -2);
        neighborCoordinates = pathFinder.NeighborTiles(baseCoordinate);
        Assert.Equal(6, neighborCoordinates.Count);
        Assert.Contains(new CubeCoordinates(1, 0, -1), neighborCoordinates);
        Assert.Contains(new CubeCoordinates(0, 1, -1), neighborCoordinates);
        Assert.Contains(new CubeCoordinates(2, 0, -2), neighborCoordinates);
        Assert.Contains(new CubeCoordinates(2, 1, -3), neighborCoordinates);
        Assert.Contains(new CubeCoordinates(1, 2, -3), neighborCoordinates);
        Assert.Contains(new CubeCoordinates(0, 2, -2), neighborCoordinates);
    }
}
