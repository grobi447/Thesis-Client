using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour
{
    public API api;
    public API.MapData map;
    public List<Tile> prefabs;
    public List<Sprite> tileAssets;
    public List<Sprite> skyAssets;
    public List<Tile> tiles = new List<Tile>();
    public List<Rail> rails = new List<Rail>();
    public Transform tilesParent;
    public Camera mainCamera;
    public Image skyboxRenderer;
    public TextMeshProUGUI mapnameText;
    public TextMeshProUGUI creatorText;
    private Dictionary<string, Tile> prefabDict;

    void Awake()
    {
        LoadJSON();
        prefabDict = new Dictionary<string, Tile>();
        foreach (var prefab in prefabs)
        {
            if (prefab != null && !string.IsNullOrEmpty(prefab.name))
            {
                prefabDict[prefab.name] = prefab;
            }
        }
        RenderMap();
        
    }

    private void LoadJSON() {
        string activeMapId = MapManager.Instance.activeMapId;
        string mapsPath = Application.dataPath + $"/Maps/{activeMapId}/{activeMapId}.json";
        string jsonContent = File.ReadAllText(mapsPath);

        var jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        map = JsonConvert.DeserializeObject<API.MapData>(jsonContent, jsonSettings);
        mapnameText.text = map.map_name;
        creatorText.text = $"Created by: {map.created_by}";
        foreach (var tileWrapper in map.data.tiles)
        {
            if (tileWrapper == null || tileWrapper.tile == null) continue;
            if (tileWrapper.trapSettings is JObject jObj)
            {
                switch (tileWrapper.tile.trapType)
                {
                    case TrapType.Spike:
                        tileWrapper.trapSettings = jObj.ToObject<SpikeSettings>();
                        break;
                    case TrapType.Saw:
                        tileWrapper.trapSettings = jObj.ToObject<SawSettings>();
                        break;
                    case TrapType.Canon:
                        tileWrapper.trapSettings = jObj.ToObject<CanonSettings>();
                        break;
                    case TrapType.Axe:
                        tileWrapper.trapSettings = jObj.ToObject<AxeSettings>();
                        break;
                    case TrapType.Blade:
                        tileWrapper.trapSettings = jObj.ToObject<BladeSettings>();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void RenderMap()
    {
        if (map == null || map.data == null)
        {
            return;
        }

        CreateParentStructure();
        RenderRails();
        RenderTiles();
        InitializeSaws();
        ApplySkybox();
    }

    private void InitializeSaws()
    {
        foreach (var tile in tiles)
        {
            if (tile is Saw saw)
            {
                saw.StartMoving();
            }
        }
    }

    private void CreateParentStructure()
    {
        if (tilesParent == null)
        {
            GameObject parentObj = new GameObject("Tiles");
            tilesParent = parentObj.transform;
        }

        for (int z = 0; z < 2; z++)
        {
            GameObject layer = new GameObject($"Layer {z}");
            layer.transform.parent = tilesParent;
        }
        GameObject boundaryParent = new GameObject("Boundaries");
        boundaryParent.transform.parent = tilesParent;

        float mapWidth = 32f;
        float mapHeight = 18f;
        GameObject boundary = new GameObject("MapBoundary");
        boundary.transform.parent = boundaryParent.transform;
        boundary.tag = "Trap";

        EdgeCollider2D edgeCollider = boundary.AddComponent<EdgeCollider2D>();
        edgeCollider.isTrigger = true;
        Vector2[] points = new Vector2[5]
        {
            new Vector2(-0.5f, -0.5f),
            new Vector2(mapWidth - 0.5f, -0.5f),
            new Vector2(mapWidth - 0.5f, mapHeight - 0.5f),
            new Vector2(-0.5f, mapHeight - 0.5f),
            new Vector2(-0.5f, -0.5f)
        };

        edgeCollider.points = points;
    }

    private void CreateBoundary(string name, Vector2 position, Vector2 size, Transform parent)
    {
        GameObject boundary = new GameObject(name);
        boundary.transform.position = position;
        boundary.transform.parent = parent;
        boundary.tag = "Block";

        BoxCollider2D collider = boundary.AddComponent<BoxCollider2D>();
        collider.size = size;
        collider.isTrigger = false;
    }

    private void RenderRails()
    {
        if (map.data.rails == null || map.data.rails.Count == 0) return;

        Rail railPrefab = GetRailPrefab();
        if (railPrefab == null)
        {
            return;
        }

        foreach (var railWrapper in map.data.rails)
        {
            Vector3 position = new Vector3(railWrapper.position.x, railWrapper.position.y, railWrapper.position.z);
            Transform parent = tilesParent.Find($"Layer {(int)position.z}");

            Rail newRail = Instantiate(railPrefab, position, Quaternion.identity, parent);
            newRail.inGame = true;
            newRail.name = $"Rail {position.x} {position.y} {position.z}";
            newRail.TileRenderer.sortingOrder = 1;
            newRail.SetSprite(railWrapper.type);
            rails.Add(newRail);
        }
    }

    private void RenderTiles()
    {
        if (map.data.tiles == null || map.data.tiles.Count == 0) return;

        foreach (var tileWrapper in map.data.tiles)
        {
            if (tileWrapper == null || tileWrapper.tile == null) continue;

            Vector3 position = new Vector3(tileWrapper.position.x, tileWrapper.position.y, tileWrapper.position.z);
            Transform parent = tilesParent.Find($"Layer {(int)position.z}");

            Tile newTile = CreateTile(tileWrapper, position, parent);
            if (newTile != null)
            {
                tiles.Add(newTile);
            }
        }
    }

    private Tile CreateTile(TileWrapper tileWrapper, Vector3 position, Transform parent)
    {
        Tile tilePrefab = GetTilePrefab(tileWrapper.tile);
        if (tilePrefab == null)
        {
            return null;
        }

        Tile newTile = Instantiate(tilePrefab, position, tilePrefab.transform.rotation, parent);
        newTile.name = tileWrapper.tile.name;
        newTile.position = position;
        newTile.inGame = true;

        Sprite tileSprite = GetSpriteByName(tileWrapper.tile.name);
        if (tileSprite != null)
        {
            newTile.TileRenderer.sprite = tileSprite;
            if(tileSprite.name.Contains("half")){
                BoxCollider2D boxCollider = newTile.GetComponent<BoxCollider2D>();
                boxCollider.size = new Vector2(boxCollider.size.x, 0.4f);
                boxCollider.offset = new Vector2(boxCollider.offset.x, 0.3f);
            }
        }

        switch (tileWrapper.tile.type)
        {
            case SpriteType.Block:
                newTile.gameObject.tag = "Block";
                break;
            case SpriteType.Trap:
                newTile.gameObject.tag = "Trap";
                break;
            case SpriteType.Spawn:
                newTile.gameObject.tag = "Spawn";
                break;
            case SpriteType.Finish:
                newTile.gameObject.tag = "Finish";
                break;
        }

        Color tileColor = newTile.TileRenderer.color;
        if (newTile.position.z == 0)
        {
            tileColor.a = 0.5f;
            newTile.GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            tileColor.a = 1f;
            if (newTile.gameObject.tag == "Spawn" || newTile.gameObject.tag == "Finish")
            {
                newTile.GetComponent<Collider2D>().isTrigger = true;
                newTile.GetComponent<Collider2D>().enabled = true;
            }
            else
            {
                newTile.GetComponent<Collider2D>().isTrigger = false;
                newTile.GetComponent<Collider2D>().enabled = true;
            }
        }
        newTile.TileRenderer.color = tileColor;

        newTile.TileRenderer.sortingOrder = (int)position.z;

        newTile.SpriteData = new SpriteData
        {
            name = tileWrapper.tile.name,
            sprite = tileSprite,
            type = tileWrapper.tile.type,
            trapType = tileWrapper.tile.trapType
        };

        if (newTile is Trap trap && tileWrapper.trapSettings != null)
        {
            ConfigureTrap(trap, tileWrapper.trapSettings);
        }

        return newTile;
    }

    private Tile GetTilePrefab(JsonSpriteData tileData)
    {
        if (tileData.type == SpriteType.Trap)
        {
            string prefabName = tileData.name;
            if (prefabDict.ContainsKey(prefabName))
            {
                return prefabDict[prefabName];
            }
        }

        if (prefabDict.ContainsKey("Tile"))
        {
            return prefabDict["Tile"];
        }

        return null;
    }

    private Rail GetRailPrefab()
    {
        if (prefabDict.ContainsKey("Rail"))
        {
            return prefabDict["Rail"] as Rail;
        }
        return null;
    }

    private Sprite GetSpriteByName(string name)
    {
        return tileAssets.FirstOrDefault(s => s != null && s.name == name);
    }

    private void ConfigureTrap(Trap trap, object trapSettings)
    {
        switch (trap.trapType)
        {
            case TrapType.Spike:
                if (trapSettings is SpikeSettings spikeSettings && trap is Spike spike)
                {
                    spike.GetComponent<BoxCollider2D>().isTrigger = true;
                    spike.settings["startTime"] = spikeSettings.startTime;
                    spike.settings["onTime"] = spikeSettings.onTime;
                    spike.settings["offTime"] = spikeSettings.offTime;
                }
                break;

            case TrapType.Saw:
                if (trapSettings is SawSettings sawSettings && trap is Saw saw)
                {
                    saw.GetComponent<BoxCollider2D>().enabled = false;
                    saw.GetComponent<CircleCollider2D>().enabled = true;
                    saw.speed = sawSettings.speed;
                }
                break;

            case TrapType.Canon:
                if (trapSettings is CanonSettings canonSettings && trap is Canon canon)
                {
                    canon.GetComponent<BoxCollider2D>().enabled = false;
                    canon.canonBody.GetComponent<BoxCollider2D>().enabled = true;
                    canon.canonType = canonSettings.canonType;
                    canon.fireRate = canonSettings.fireRate;
                    canon.projectileSpeed = canonSettings.projectileSpeed;
                    canon.targetingPlayer = canonSettings.targetingPlayer;
                }
                break;

            case TrapType.Axe:
                if (trapSettings is AxeSettings axeSettings && trap is Axe axe)
                {
                    axe.GetComponent<BoxCollider2D>().enabled = false;
                    axe.GetComponent<CapsuleCollider2D>().enabled = true;
                    axe.axeMovement = axeSettings.axeMovement;
                    axe.axeDirection = axeSettings.axeDirection;
                    axe.speed = axeSettings.speed;
                }
                break;

            case TrapType.Blade:
                if (trapSettings is BladeSettings bladeSettings && trap is Blade blade)
                {
                    blade.GetComponentInParent<BoxCollider2D>().enabled = false;
                    blade.tileBoxCollider.enabled = true;
                    blade.settings["crushTime"] = bladeSettings.crushTime;
                    blade.settings["upTime"] = bladeSettings.upTime;
                    blade.settings["reload"] = bladeSettings.reload;
                }
                break;
        }
    }

    public Rail GetRailAtPosition(Vector3 position)
    {
        return rails.FirstOrDefault(r => 
            Mathf.Approximately(r.transform.position.x, position.x) && 
            Mathf.Approximately(r.transform.position.y, position.y) &&
            Mathf.Approximately(r.transform.position.z, position.z));
    }

    public bool HasTileAtPosition(Vector3 position)
    {
        Tile tile = tiles.FirstOrDefault(t =>
            Mathf.Approximately(t.transform.position.x, position.x) &&
            Mathf.Approximately(t.transform.position.y, position.y) &&
            Mathf.Approximately(t.transform.position.z, position.z));
        
        return tile != null && tile.SpriteData != null && tile.SpriteData.type == SpriteType.Block;
    }

    private void ApplySkybox()
    {
        if (string.IsNullOrEmpty(map.data.skybox)) return;

        Sprite skySprite = skyAssets.FirstOrDefault(s => s != null && s.name == map.data.skybox);
        if (skySprite != null && skyboxRenderer != null)
        {
            skyboxRenderer.sprite = skySprite;
        }
    }
}
