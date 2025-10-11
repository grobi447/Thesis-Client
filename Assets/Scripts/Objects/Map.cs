using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class Map
{

    public string skybox;
    public MapMetaData metaData;
    public List<TileWrapper> tiles;
    public List<RailWrapper> rails;

    public Map(Dictionary<Vector3, Tile> tiles, List<Rail> rails, string skybox)
    {
        this.metaData = new MapMetaData
        {
            id = Guid.NewGuid().ToString(),
            name = "New Map",
            creator = UserManager.Instance.username
        };
        this.tiles = tiles.Where(t => t.Value.SpriteData != null)
                           .Select(t => new TileWrapper(t.Key, t.Value))
                           .ToList();
        this.rails = rails.Select(r => new RailWrapper(r.position, r))
                          .ToList();
        this.skybox = skybox;
    }
}

[Serializable]
public class TileWrapper
{
    public Vector3 position;
    public SpriteData tile;

    public TileWrapper(Vector3 position, Tile tile)
    {
        this.position = position;
        this.tile = tile.SpriteData;
    }
}
[Serializable]
public class RailWrapper
{
    public Vector3 position;
    public RailBitmapType type;

    public RailWrapper(Vector3 position, Rail rail)
    {
        this.position = position;
        this.type = rail.currentType;
    }
}
[Serializable]
public class MapMetaData
{
    public string id;
    public string name;
    public string creator;
}
