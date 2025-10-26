using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class Map
{

    public string skybox;
    public List<TileWrapper> tiles;
    public List<RailWrapper> rails;

    public Map()
    {
        tiles = new List<TileWrapper>();
        rails = new List<RailWrapper>();
    }

    public Map(Dictionary<Vector3, Tile> tiles, List<Rail> rails, string skybox)
    {
        this.tiles = tiles
            .Where(t => t.Value.SpriteData != null)
            .Select(t => new TileWrapper(t.Key, t.Value))
            .ToList();
        this.rails = rails
            .Select(r => new RailWrapper(r.position, r))
            .ToList();
        this.skybox = skybox;
    }
}

[Serializable]
public class TileWrapper
{
    public Vec3 position;
    public JsonSpriteData tile;
    [SerializeReference]
    public object trapSettings;

    public TileWrapper()
    {
    }

    public TileWrapper(Vector3 position, Tile tile)
    {
        this.position = new Vec3(position);
        this.tile = new JsonSpriteData(tile.SpriteData);
        this.trapSettings = CreateTrapSettings(tile);
    }

    private static object CreateTrapSettings(Tile tile)
    {
        if (tile == null || tile.SpriteData == null)
            return null;

        switch (tile.SpriteData.trapType)
        {
            case TrapType.Axe:
                return new AxeSettings((Axe)tile);
            case TrapType.Blade:
                return new BladeSettings((Blade)tile);
            case TrapType.Canon:
                return new CanonSettings((Canon)tile);
            case TrapType.Saw:
                return new SawSettings((Saw)tile);
            case TrapType.Spike:
                return new SpikeSettings((Spike)tile);
            default:
                return null;
        }
    }
}

[Serializable]
public class RailWrapper
{
    public Vec3 position;
    public RailBitmapType type;

    public RailWrapper()
    {
    }

    public RailWrapper(Vector3 position, Rail rail)
    {
        this.position = new Vec3(position);
        this.type = rail.currentType;
    }
}

[Serializable]
public struct Vec3
{
    public float x;
    public float y;
    public float z;

    public Vec3(Vector3 v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }
}

[Serializable]
public class JsonSpriteData
{
    public string name;
    public SpriteType type;
    public TrapType trapType;

    public JsonSpriteData()
    {
    }

    public JsonSpriteData(SpriteData data)
    {
        if (data == null)
        {
            this.name = null;
            this.type = default;
            this.trapType = default;
            return;
        }
        this.name = data.name;
        this.type = data.type;
        this.trapType = data.trapType;
    }
}

[Serializable]
public class AxeSettings
{
    public AxeMovement axeMovement;
    public AxeDirection axeDirection;
    public float speed;

    public AxeSettings()
    {
    }

    public AxeSettings(Axe axe)
    {
        this.axeMovement = axe.axeMovement;
        this.axeDirection = axe.axeDirection;
        this.speed = axe.speed;
    }
}

[Serializable]
public class BladeSettings
{
    public float crushTime;
    public float upTime;
    public float reload;

    public BladeSettings()
    {
    }

    public BladeSettings(Blade blade)
    {
        this.crushTime = blade.settings["crushTime"];
        this.upTime = blade.settings["upTime"];
        this.reload = blade.settings["reload"];
    }
}

[Serializable]
public class CanonSettings
{
    public CanonType canonType;
    public float fireRate;
    public float projectileSpeed;

    public CanonSettings()
    {
    }

    public CanonSettings(Canon canon)
    {
        this.canonType = canon.canonType;
        this.fireRate = canon.fireRate;
        this.projectileSpeed = canon.projectileSpeed;
    }
}

[Serializable]
public class SawSettings
{
    public float speed;

    public SawSettings()
    {
    }

    public SawSettings(Saw saw)
    {
        this.speed = saw.speed;
    }
}

[Serializable]
public class SpikeSettings
{
    public float startTime;
    public float onTime;
    public float offTime;

    public SpikeSettings()
    {
    }

    public SpikeSettings(Spike spike)
    {
        this.startTime = spike.settings["startTime"];
        this.onTime = spike.settings["onTime"];
        this.offTime = spike.settings["offTime"];
    }
}