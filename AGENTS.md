# AGENTS.md - AI Agent Context Documentation

**Project**: hex-map-pathfinder  
**Version**: 0.5.0  
**Owner**: Ziagl (Werner Ziegelwanger / Hexagon Simulations)  
**Language**: C# (.NET 10.0)  
**Last Updated**: November 25, 2025

## Project Overview

**hex-map-pathfinder** is a C# pathfinding library optimized for hexagonal grid-based games and simulations. It implements the A* algorithm with support for:
- Variable terrain movement costs
- Multi-layer maps (land/sea/air)
- Special tile properties (rivers with crossing mechanics)
- Reachability analysis for movement ranges
- Combat range calculations

**Key Dependency**: `HexMapBase` v0.4.1 - Provides hex coordinate systems and grid utilities

**Target Frameworks**: .NET 10.0 (primary), .NET 8.0 (legacy support)

## Architecture & Project Structure

```
HexMapPathFinder/                   # Main library (NuGet package)
├── PathFinder.cs                   # Public API - Main entry point (~500 lines)
├── Models/
│   ├── MapData.cs                 # Map configuration (internal record)
│   ├── Tile.cs                    # A* node with cost tracking (internal)
│   ├── TileProperty.cs            # Special tile behaviors (public enum)
│   └── Utils.cs                   # Helper functions (internal static class)
└── HexMapPathFinder.csproj

HexMapPathFinder.Tests/             # MSTest test suite
├── PathFinderTests.cs             # Main functionality tests (13 methods)
├── PathFinderSerializationTests.cs # JSON/Binary serialization tests
└── UtilsTests.cs                  # Internal utility tests
```

**Namespace**: `HexagonSimulations.HexMapPathfinder`

**Internal Visibility**: Tests have `InternalsVisibleTo` access for verifying internal components

## Key Classes & Components

### `PathFinder` (Public API)

Main class exposing all pathfinding functionality. Users interact exclusively with this class.

**Constructor**:
```csharp
PathFinder(List<List<int>> costMap, List<List<TileProperty>> propertyMap, int rows, int columns)
PathFinder(List<List<int>> costMap, int rows, int columns)  // Overload without properties
```

**Critical Public Methods**:

| Method | Purpose | Parameters | Returns |
|--------|---------|------------|---------|
| `ComputePath` | A* shortest path | start, end (CubeCoordinates), layerIndex | `List<HexTile>` |
| `ComputePathOffsetCoordinates` | A* with 2D coords | start, end (OffsetCoordinates), layerIndex | `List<HexTile>` |
| `ReachableTiles` | All tiles within range | start, maxCost, layerIndex, obstacles (blocking/non-blocking) | `List<HexTile>` |
| `ReachableTilesOffsetCoordinates` | Same with 2D coords | start, maxCost, layerIndex, obstacles | `List<HexTile>` |
| `InRange` | Attack/combat range | start, targets, maxCost, layerIndex, obstacles | `List<HexTile>` |
| `Neighbors` | Adjacent tiles | position, layerIndex | `List<HexTile>` |
| `PathToWeightedPath` | Add costs to path | path, layerIndex | `List<WeightedHexTile>` |

**Serialization Support**:
- `SerializeMapData()` → JSON string
- `DeserializeMapData(string json)` → Reconstructs PathFinder
- `SerializeMapDataBinary()` → byte array
- `DeserializeMapDataBinary(byte[] data)` → Reconstructs PathFinder

**Key Implementation Details**:
- Uses `MAX_LOOP_COUNT = 100000` to prevent infinite loops in pathfinding
- Maintains `_openList` (frontier) and `_closedList` (explored) for A* algorithm
- Shuffles neighbors during exploration to avoid deterministic tie-breaking artifacts
- Cost map: `0` = impassable, `1+` = movement cost
- Supports multiple layers via `List<List<int>>` structure (flattened 2D arrays)

### `MapData` (Internal Record)

Immutable data structure holding map configuration:

```csharp
record MapData
{
    List<List<int>> Map;                      // Cost layers
    List<List<TileProperty>> PropertyMap;     // Special tile properties per layer
    int Rows;
    int Columns;
}
```

**Serialization**: JSON via `System.Text.Json` with `JsonStringEnumConverter` for TileProperty enums

### `Tile` (Internal Class)

Extends `HexTile` from HexMapBase with A* specific fields:

```csharp
class Tile : HexTile
{
    int MovementCost;              // g-cost: Accumulated cost from start
    int EstimatedMovementCost;     // h-cost: Heuristic distance to goal
    int Sum;                       // f-cost: g + h (determines priority)
}
```

**Critical for A* algorithm**: Lower `Sum` values are explored first.

### `TileProperty` (Public Enum)

Special tile behaviors that modify pathfinding rules:

```csharp
enum TileProperty
{
    NONE = 0,
    RIVER = 1,      // Water tile - special crossing rules
    RIVERBANK = 2,  // Shore tile - allows entry to rivers
}
```

**River Crossing Logic**:
- Movement from `RIVER` ↔ `RIVERBANK` requires starting adjacent to the river
- Otherwise, river tiles have `int.MaxValue` cost (effectively impassable)
- Simulates needing to be at water's edge to cross
- Implemented in `ComputePath` with special adjacency checks

### `Utils` (Internal Static Class)

Helper functions marked with `[assembly: InternalsVisibleTo]`:

- `GetWalkableNeighbors()` - Filters neighbors by cost > 0
- `Shuffle<T>()` - Randomizes list order using Fisher-Yates algorithm

## Algorithms & Data Structures

### A* Pathfinding Algorithm

**Implementation Flow**:
1. Initialize start tile with g-cost = 0, add to open list
2. Main loop (with `MAX_LOOP_COUNT` protection):
   - Pop tile with lowest f-cost from open list
   - Add to closed list
   - If destination reached → reconstruct path backwards
   - Get walkable neighbors (shuffled)
   - For each neighbor:
     - Skip if in closed list
     - Calculate g-cost = current.MovementCost + tile cost
     - Calculate h-cost = hex distance heuristic
     - Add/update in open list with f-cost = g + h
3. Path reconstruction: Follow tiles backwards from end to start

**Heuristic**: Uses `CubeCoordinates.Distance()` from HexMapBase (Manhattan distance in cube space)

**Key Optimizations**:
- Early exit if start tile is impassable (cost == 0)
- Neighbor shuffling prevents pathfinding artifacts from exploration order
- Closed list prevents revisiting explored tiles

### Reachable Tiles (Modified Dijkstra)

Similar to A* but without specific destination:
- Explores all tiles within `maxCost` budget
- Doesn't use heuristic (h-cost always 0)
- Returns all tiles in closed list
- Supports two obstacle types:
  - **Non-blocking obstacles**: Can path through but not stop on (e.g., friendly units)
  - **Blocking obstacles**: Completely block pathfinding (e.g., enemies)

### Coordinate Systems

Uses HexMapBase coordinate types:
- **`CubeCoordinates`**: (q, r, s) where q + r + s = 0 (best for hex math)
- **`OffsetCoordinates`**: (x, y) traditional 2D coordinates
- Conversion: `CubeCoordinates.ToCube(offset, rows, columns)` and vice versa

**Grid Indexing**: Flattened 2D array access:
```csharp
int index = offsetY * columns + offsetX;
int cost = costMap[layerIndex][index];
```

## Testing Approach

### Test Philosophy
- Small predictable grids (typically 4x4) for exact verification
- Exact count assertions (not approximate)
- Multi-run tests for randomization validation
- Regression tests for past bugs

### Key Test Patterns in `PathFinderTests.cs`

**Path Computation** (7 tests):
- `TestComputePath*` - Basic pathfinding with obstacles
- `TestComputePathBlocked*` - Verifies obstacles block correctly (runs 10 times)
- `TestComputePathTileCosts*` - Variable terrain costs
- `TestComputePathEmpty*` - Handles unreachable destinations

**Reachability** (3 tests):
- `TestReachableTiles*` - Movement range with/without obstacles
- `TestReachableTilesBug*` - Regression test for distance > 2 bug

**Special Features** (3 tests):
- `TestNeighbors` - Edge detection (2 neighbors at corner vs 6 in middle)
- `TestPathToWeightedPath` - Cost accumulation
- `TestRiverProperty` - River crossing with 4 scenarios

**Example Test Structure**:
```csharp
[TestMethod]
public void TestComputePath()
{
    // Arrange: 4x4 grid with obstacles (cost = 0)
    List<int> costMap = new() { 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1 };
    var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
    
    // Act: Compute path avoiding obstacles
    var path = pathFinder.ComputePath(
        new CubeCoordinates(0, 0, 0),
        new CubeCoordinates(2, 3, -5),
        0
    );
    
    // Assert: Exact path length
    Assert.AreEqual(7, path.Count);
}
```

### Serialization Tests in `PathFinderSerializationTests.cs`

- Round-trip JSON serialization with normalized comparison
- Round-trip binary serialization
- Optional disk dump for debugging (`DUMP_SERIALIZATION_TO_DISK = false`)

## Common Modification Scenarios

### Adding New Tile Properties

1. **Add enum value** to `TileProperty` in `Models/TileProperty.cs`
2. **Implement logic** in `PathFinder.ComputePath()` method where river logic exists (search for `TileProperty.RIVER`)
3. **Add tests** in `PathFinderTests.cs` following `TestRiverProperty` pattern
4. **Update serialization tests** if property affects pathfinding

**Example Pattern**:
```csharp
// In ComputePath, check property before adding to open list
if (propertyMap != null && propertyMap[layerIndex][index] == TileProperty.NEW_TYPE)
{
    // Special handling logic
}
```

### Extending Pathfinding Algorithms

Key methods to modify:
- `ComputePath()` - Core A* implementation
- `ReachableTiles()` - Movement range calculation
- `InRange()` - Combat range logic

**Important**: Maintain `MAX_LOOP_COUNT` protection and neighbor shuffling.

### Multi-Layer Map Support

Current support: Multiple layers via `List<List<int>>` structure
- Each layer is a separate flattened 2D array
- `layerIndex` parameter selects active layer
- Property maps align with cost maps by layer

**To add layer interactions**: Modify neighbor calculation to include vertical movement between layers.

### Modifying Cost Calculation

Cost logic is in `ComputePath()`:
```csharp
newCost = node.MovementCost + tiles[layerIndex][nextNode.GridIndex].MovementCost;
```

To change:
1. Modify cost accumulation formula
2. Update `PathToWeightedPath()` to match
3. Add tests verifying new cost model

## Code Conventions & Patterns

### Design Patterns Used
- **Facade Pattern**: `PathFinder` simplifies complex A* internals
- **Builder Pattern**: Multi-step initialization (cost map → property map → pathfinder)
- **Strategy Pattern**: Different obstacle handling (blocking vs non-blocking)
- **Record Type**: `MapData` for immutable configuration

### Naming Conventions
- **Public API**: PascalCase methods (`ComputePath`, `ReachableTiles`)
- **Internal classes**: PascalCase with internal visibility
- **Private fields**: `_camelCase` with underscore prefix
- **Constants**: UPPER_SNAKE_CASE (`MAX_LOOP_COUNT`)

### Error Handling
- Returns empty collections for invalid inputs (not exceptions)
- Null checks for optional parameters (obstacles, property maps)
- Layer bounds checking with default fallback
- Loop protection prevents hangs

## API Reference Quick Guide

### Typical Usage Pattern

```csharp
// 1. Create cost map (0 = blocked, 1+ = cost)
List<int> costMap = new() { 1, 1, 2, 3, 1, 2, 1, 3, 2, 4, 8, 1, 3, 1, 2, 1 };

// 2. Optional: Create property map for special tiles
List<TileProperty> propertyMap = new() { 
    TileProperty.NONE, TileProperty.RIVER, /* ... */ 
};

// 3. Initialize PathFinder
var pathFinder = new PathFinder(
    new List<List<int>>() { costMap },           // Multiple layers supported
    new List<List<TileProperty>>() { propertyMap }, // Optional
    rows: 4, 
    columns: 4
);

// 4. Find shortest path
var path = pathFinder.ComputePath(
    start: new CubeCoordinates(0, 0, 0),
    end: new CubeCoordinates(2, 3, -5),
    layerIndex: 0
);

// 5. Find reachable tiles (movement range)
var reachable = pathFinder.ReachableTiles(
    start: new CubeCoordinates(0, 0, 0),
    maxCost: 6,
    layerIndex: 0
);

// 6. Find enemies in attack range
var targets = pathFinder.InRange(
    start: new CubeCoordinates(0, 0, 0),
    targets: enemyPositions,  // List<HexTile>
    maxCost: 3,
    layerIndex: 0
);
```

### Return Value Expectations
- Empty list if no path/tiles found (not null)
- Path includes start and end tiles
- Reachable tiles includes starting position
- Weighted path preserves order, adds costs

## Dependencies & Build Configuration

### NuGet Dependencies
- **HexMapBase** v0.4.1 - Core hex grid library providing:
  - `HexTile`, `CubeCoordinates`, `OffsetCoordinates`
  - `HexGrid.InitializeGrid<T>()`
  - `HexGrid.GetAllNeighbors()`
  - Coordinate conversion utilities

### Test Dependencies
- Microsoft.NET.Test.Sdk v18.0.0
- MSTest.TestAdapter v3.11.0
- MSTest.TestFramework v3.11.0

### Project Configuration
- Target Framework: .NET 10.0 (with .NET 8.0 legacy support)
- Nullable Reference Types: Enabled
- Implicit Usings: Enabled
- Output Type: Library (NuGet package)

### NuGet Package Metadata
- **Package ID**: HexMapPathFinder
- **Title**: Hexagon Pathfinding Library
- **Tags**: hexagon, geometry, map, path finding, astar, tile, 2d, library
- **Icon**: `hex_map_pathfinder_package_icon.png` (included in package)
- **License**: LICENSE file (included in package)

## Important Constraints & Limitations

### Performance Constraints
- `MAX_LOOP_COUNT = 100000` - Hard limit on pathfinding iterations
- Large maps (>100x100) may hit iteration limit with complex paths
- Consider increasing limit for larger maps or pre-validating reachability

### API Limitations
- Single-layer pathfinding (no vertical layer transitions in one path)
- Layer index must be valid or defaults to 0
- Obstacle lists are optional (null allowed)
- Property map must align with cost map dimensions if provided

### Special Tile Behaviors
- River crossing requires specific adjacency (can't teleport into rivers)
- Only RIVER and RIVERBANK properties implemented
- Extending properties requires code changes (not data-driven)

## Testing Commands

```powershell
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "ClassName=PathFinderTests"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run tests with coverage (if configured)
dotnet test /p:CollectCoverage=true
```

## Future Agent Guidance

### When Modifying Pathfinding Logic
1. Always maintain `MAX_LOOP_COUNT` protection
2. Keep neighbor shuffling to avoid artifacts
3. Update both `CubeCoordinates` and `OffsetCoordinates` versions
4. Add corresponding tests for new behavior
5. Verify serialization still works if changing `MapData`

### When Adding Features
1. Check if feature belongs in `PathFinder` (public) or `Utils` (internal)
2. Consider multi-layer impact
3. Add property enum if tile-specific behavior needed
4. Follow existing test patterns (small grids, exact assertions)
5. Update this AGENTS.md file with new patterns

### When Debugging
1. Check test files first - they demonstrate expected behavior
2. Use `PathFinderSerializationTests` with `DUMP_SERIALIZATION_TO_DISK = true` to inspect state
3. Verify coordinate system (cube vs offset)
4. Check layer index bounds
5. Confirm cost map layout matches grid dimensions

### Code Review Checklist
- [ ] Public API additions are properly documented
- [ ] Tests added for new functionality
- [ ] Both coordinate system variants implemented
- [ ] Serialization compatibility maintained
- [ ] Internal visibility appropriate
- [ ] No breaking changes to existing API
- [ ] Performance impact considered (iteration limits)

## Additional Resources

**Example Images**: See `example_images/` directory for visual demonstrations of:
- `reachableTiles.png` - Movement range visualization
- `computePath.png` - Shortest path example
- `computePath2.png` - Pathfinding around obstacles

**HexMapBase Documentation**: Refer to dependency documentation for hex grid utilities and coordinate systems

**A* Algorithm**: Standard A* with hex-specific heuristic (Manhattan distance in cube coordinates)
