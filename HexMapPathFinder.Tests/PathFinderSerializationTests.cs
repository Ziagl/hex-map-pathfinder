using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapPathfinder;
using com.hexagonsimulations.HexMapPathfinder.Models;

namespace com.hexagonsimulations.HexMapPathFinder.Tests;

[TestClass]
public sealed class PathFinderSerializationTests
{
    private readonly string TempDir = @"C:\Temp\";
    private readonly bool DumpToDisk = false; // set to true to dump serialized data to disk for inspection

    [TestMethod]
    public void PathFinder_Json()
    {
        var pathFinder = ExamplePathFinder();

        var json = pathFinder.ToJson();
        Assert.IsFalse(string.IsNullOrWhiteSpace(json), "JSON should not be empty.");

        if (DumpToDisk)
        {
            File.WriteAllText($"{TempDir}PathFinder.json", json);
        }

        var roundTripped = PathFinder.FromJson(json);
        Assert.IsNotNull(roundTripped, "Deserialized PathFinder should not be null.");

        AssertPathFinderEqual(pathFinder, roundTripped);
    }

    [TestMethod]
    public void PathFinder_Binary()
    {
        var pathFinder = ExamplePathFinder();

        using var ms = new MemoryStream();
        using (var writer = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
        {
            pathFinder.Write(writer);
        }

        if (DumpToDisk)
        {
            File.WriteAllBytes($"{TempDir}PathFinder.bin", ms.ToArray());
        }

        ms.Position = 0;
        PathFinder roundTripped;
        using (var reader = new BinaryReader(ms, System.Text.Encoding.UTF8, leaveOpen: true))
        {
            roundTripped = PathFinder.Read(reader);
        }

        Assert.IsNotNull(roundTripped, "Binary deserialized PathFinder should not be null.");
        AssertPathFinderEqual(pathFinder, roundTripped);
    }

    private PathFinder ExamplePathFinder()
    {
        List<int> costMap = new() { 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1 };
        var grid = HexGrid.InitializeGrid<Tile>(4, 4);
        HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        return new PathFinder(new List<List<int>>() { costMap }, 4, 4);
    }

    private static void AssertPathFinderEqual(PathFinder expected, PathFinder actual)
    {
        Assert.IsNotNull(expected, "Expected PathFinder should not be null.");
        Assert.IsNotNull(actual, "Actual PathFinder should not be null.");

        var expectedJson = expected.ToJson();
        var actualJson = actual.ToJson();

        Assert.IsFalse(string.IsNullOrWhiteSpace(expectedJson), "Expected JSON should not be empty.");
        Assert.IsFalse(string.IsNullOrWhiteSpace(actualJson), "Actual JSON should not be empty.");

        // Normalize and compare JSON to ensure full structural equality.
        Assert.AreEqual(NormalizeJson(expectedJson), NormalizeJson(actualJson), "PathFinder instances differ (JSON mismatch).");
    }

    private static string NormalizeJson(string json)
    {
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        return System.Text.Json.JsonSerializer.Serialize(doc, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = false
        });
    }
}