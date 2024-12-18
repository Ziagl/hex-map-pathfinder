namespace hex_map_pathfinder.Models;

internal record MapData
{
    public List<List<int>> Map { get; set; } = new();           // general cost map (0 not passable, 1-? costs to pass field)
    public List<List<TileProperty>> PropertyMap { get; set; } = new();   // additional propert map (f.e. river for land, deep or shallow water, ...)
    public int Rows { get; set; }
    public int Columns { get; set; }
}
