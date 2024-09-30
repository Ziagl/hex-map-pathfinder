# hex-map-pathfinder
A path finding library for hexagon maps.

## Sample images of usage in possible environment

This image shows how reachableTiles is used on a map with different tiles (tiles with different movement costs). In this example reachableTiles computes all tiles that can be reached with movement value of 6 (up to 6 tiles). Plain and Sand costs 1, Wood and Hills costs 2, Mountains are unpassable.
![Alt text](example_images/reachableTiles.png?raw=true "Reachable Tiles example")

With computePath it is possible to find shortest (with the lowest costs) path.
![Alt text](example_images/computePath.png?raw=true "Compute Path example")

It ignores tiles that are not passable. If no path is possible, it returns an empty path.
![Alt text](example_images/computePath2.png?raw=true "Different Compute Path example")

## Sample code

```csharp
List<int> costMap = new() { 1, 1, 2, 3, 1, 2, 1, 3, 2, 4, 8, 1, 3, 1, 2, 1 };
var grid = HexGrid.InitializeGrid<Tile>(4, 4);
HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
var reachableTiles = pathFinder.ReachableTiles(new CubeCoordinates(0, 0, 0), 2, 0);
// reachableTiles.length is 7
```

Example usage of ReachableTiles. It is important to initialize the component with a heatmap (a 2D map of all tiles used with movement value 0 -> not passable, >1 cost for passing). reachableTiles is called with a starting coordinate in cube coordinate system and a max cost value.

```csharp
List<int> costMap = new() { 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1 };
var grid = HexGrid.InitializeGrid<Tile>(4, 4);
HexTile tile = new HexTile() { Coordinates = new CubeCoordinates(0, 0, 0) };
var pathFinder = new PathFinder(new List<List<int>>() { costMap }, 4, 4);
var path = pathFinder.ComputePath(new CubeCoordinates(0, 0, 0), new CubeCoordinates(2, 3, -5), 0);
// path.length is 7
```

Example usage of ComputePath. Compute path is called with 2 cube coordinates for start and end. A second version computePathOffsetCoordinates does the same, but start and end can be passed as offset coordinates (normal x and y values in 2D space). It returns all tiles of shortest path.