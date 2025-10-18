using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.hexagonsimulations.HexMapPathfinder.Models;

internal record MapData
{
    public List<List<int>> Map { get; set; } = new();                    // general cost map (0 not passable, 1-? costs to pass field)
    public List<List<TileProperty>> PropertyMap { get; set; } = new();   // additional property map (e.g. river for land, deep or shallow water, ...)
    public int Rows { get; set; }
    public int Columns { get; set; }

    // JSON options for serialization
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// Serialize this MapData instance to a JSON string.
    /// </summary>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, JsonOptions);
    }

    /// <summary>
    /// Deserialize a JSON string into a MapData instance.
    /// </summary>
    public static MapData FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON string cannot be null or empty.", nameof(json));

        return JsonSerializer.Deserialize<MapData>(json, JsonOptions)
                      ?? throw new InvalidOperationException("Failed to deserialize MapData: result was null.");
    }

    /// <summary>
    /// Write this MapData instance into a binary stream.
    /// Layout:
    /// int Rows, int Columns,
    /// Rows * Columns ints for Map,
    /// Rows * Columns ints for PropertyMap (enum underlying values).
    /// </summary>
    public void Write(BinaryWriter writer)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }
        if (Rows <= 0 || Columns <= 0)
        {
            throw new InvalidOperationException("Rows and Columns must be greater than zero before writing.");
        }

        // Basic dimensions
        writer.Write(Rows);
        writer.Write(Columns);
        writer.Write(Map.Count);

        // Write Map values
        for (int i = 0; i < Map.Count; i++)
        {
            var map = Map[i];
            for (int j = 0; j < map.Count; j++)
            {
                writer.Write(map[j]);
            }
        }

        writer.Write(PropertyMap.Count);

        // Write properties
        for (int i = 0; i < PropertyMap.Count; i++)
        {
            var map = PropertyMap[i];
            for (int j = 0; j < map.Count; j++)
            {
                writer.Write((int)map[j]);
            }
        }
    }

    /// <summary>
    /// Read data written by Write(). Resulting structure uses flattened single layer for Map / PropertyMap.
    /// </summary>
    public static MapData Read(BinaryReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        // Read dimensions
        int rows = reader.ReadInt32();
        int columns = reader.ReadInt32();
        int mapCount = reader.ReadInt32();

        if (rows <= 0 || columns <= 0 || mapCount <= 0)
        {
            throw new InvalidOperationException("Invalid data: Rows, Columns, and Map count must be greater than zero.");
        }

        // Read Map values
        var map = new List<List<int>>(mapCount);
        for (int i = 0; i < mapCount; i++)
        {
            var row = new List<int>(rows * columns);
            for (int j = 0; j < rows * columns; j++)
            {
                row.Add(reader.ReadInt32());
            }
            map.Add(row);
        }

        int propertyMapCount = reader.ReadInt32();

        // Read PropertyMap values
        var propertyMap = new List<List<TileProperty>>(propertyMapCount);
        for (int i = 0; i < propertyMapCount; i++)
        {
            var row = new List<TileProperty>(rows * columns);
            for (int j = 0; j < rows * columns; j++)
            {
                row.Add((TileProperty)reader.ReadInt32());
            }
            propertyMap.Add(row);
        }

        return new MapData
        {
            Rows = rows,
            Columns = columns,
            Map = map,
            PropertyMap = propertyMap
        };
    }
}
