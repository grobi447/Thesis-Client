using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.CompilerServices;
using System;
using System.IO;
using Newtonsoft.Json;

public class GridManager : MonoBehaviour
{
    [SerializeField] private API api;
    [SerializeField] private int width, height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private List<Tile> trapTilePrefabs;
    [SerializeField] private Transform cam;
    [SerializeField] private SpriteData selectedSpriteData;
    [SerializeField] private Transform gridParent;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private UiHandler uiHandler;
    [SerializeField] private Settings settings;
    [SerializeField] public Rail railPrefab;
    public List<Rail> rails = new List<Rail>();
    public HashSet<Trap> activeTraps = new HashSet<Trap>();
    public HashSet<Trap> allTraps = new HashSet<Trap>();
    public static bool isMouseDown = false;
    public static bool hasSelectedSprite = false;
    public static int currentLayer = 1;
    public CanonType currentCanonDirection = CanonType.Left;
    public AxeMovement currentAxeMovement = AxeMovement.Half;
    public AxeDirection currentAxeDirection = AxeDirection.Down;
    private Dictionary<Vector3, Tile> tiles;
    private Dictionary<string, Tile> trapPrefabDict;
    public Tile TilePrefab => tilePrefab;
    public Dictionary<string, Tile> TrapPrefabDict => trapPrefabDict;

    public Dictionary<View, SpriteData> lastSelectedSprite = new Dictionary<View, SpriteData>
    {
        { View.Blocks, null },
        { View.Traps, null },
        { View.Sky, null }
    };

    private void Awake()
    {
        trapPrefabDict = new Dictionary<string, Tile>();
        foreach (var prefab in trapTilePrefabs)
            trapPrefabDict.Add(prefab.name, prefab);
    }

    private void Start()
    {
        GameObject parentObj = GameObject.Find("Tiles");
        gridParent = parentObj.transform;
        GenerateGrid();
    }

    private void Update()
    {
        isMouseDown = Input.GetMouseButton(0);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateMap();
        }

    }

    private void GenerateGrid()
    {
        tiles = new Dictionary<Vector3, Tile>();
        for (int z = 0; z < 2; z++)
        {
            GameObject layer = new GameObject($"Layer {z}");
            layer.transform.parent = gridParent;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tile newTile = Instantiate(tilePrefab, new Vector3(x, y, z), Quaternion.identity, layer.transform);
                    newTile.name = $"Tile {x} {y} {z}";
                    newTile.Init(this, new Vector3(x, y, z));
                    newTile.TileRenderer.sortingOrder = z;
                    tiles[new Vector3(x, y, z)] = newTile;
                }
            }
        }
        UpdateColliders();
    }

    public void SetSelectedSprite(View view, SpriteData data)
    {
        if (data == null) return;
        selectedSpriteData = data;
        hasSelectedSprite = true;
        lastSelectedSprite[view] = data;
    }

    public SpriteData GetLastSelectedSprite(View view) => lastSelectedSprite[view];

    public void CheckActiveToggle()
    {
        Toggle activeToggle = toggleGroup.ActiveToggles().FirstOrDefault();
        if (activeToggle.name == "Background")
            currentLayer = 0;
        else if (activeToggle.name == "Playfield")
            currentLayer = 1;
        UpdateColliders();
    }

    public void UpdateColliders()
    {
        for (int z = 0; z < 2; z++)
        {
            Transform layer = gridParent.Find($"Layer {z}");
            foreach (Transform tile in layer)
            {
                if (z == currentLayer)
                    tile.GetComponent<Tile>().TurnOnCollider();
                else
                    tile.GetComponent<Tile>().TurnOffCollider();
            }
        }
    }

    public int GetCurrentLayer() => currentLayer;

    public void ReplaceToTrap(SpriteData trapData, Vector3 position)
    {
        if (tiles.TryGetValue(position, out Tile oldTile))
        {
            if (oldTile is Trap oldTrap)
            {
                allTraps.Remove(oldTrap);
            }
            Transform parent = oldTile.transform.parent;
            DestroyImmediate(oldTile.gameObject);

            if (trapPrefabDict.TryGetValue(trapData.name, out Tile trapPrefab))
            {
                Tile trapTile = Instantiate(trapPrefab, position, Quaternion.identity, parent);
                trapTile.name = trapData.name;
                trapTile.Init(this, position);
                trapTile.transform.rotation = trapPrefab.transform.rotation;
                trapTile.TileRenderer.sortingOrder = (int)position.z;
                trapTile.SpriteData = trapData;
                trapTile.TileRenderer.sprite = trapData.sprite;
                trapTile.emptySprite = trapPrefab.emptySprite;
                trapTile.tag = trapData.type.ToString();
                tiles[position] = trapTile;
                allTraps.Add((Trap)trapTile);
                if (trapTile is Saw saw)
                {
                    saw.StartMoving();
                }
                trapTile.UseTool();
            }
        }
    }

    public void ReplaceToTile(SpriteData blockData, Vector3 position)
    {
        if (blockData == null) blockData = new SpriteData { name = "Empty", sprite = tilePrefab.emptySprite, type = SpriteType.Empty };
        if (tiles.TryGetValue(position, out Tile oldTile))
        {
            Transform parent = oldTile.transform.parent;
            DestroyImmediate(oldTile.gameObject);

            Tile newTile = Instantiate(tilePrefab, position, Quaternion.identity, parent);
            newTile.name = blockData.name;
            newTile.Init(this, position);
            newTile.TileRenderer.sortingOrder = (int)position.z;
            newTile.SpriteData = blockData;
            newTile.TileRenderer.sprite = blockData.sprite;
            newTile.emptySprite = tilePrefab.emptySprite;
            newTile.tag = blockData.type == SpriteType.Empty ? "Untagged" : blockData.type.ToString();
            tiles[position] = newTile;
            if (oldTile is Trap) allTraps.Remove((Trap)oldTile);
            if (oldTile is Rail)
            {
                rails.Remove((Rail)oldTile);
            }
            newTile.UseTool();
        }
    }

    public bool HasBlockNeighbor(Tile tile, Vector3 direction)
    {
        Vector3 neighborPos = tile.transform.position + direction;
        neighborPos = new Vector3(
            Mathf.Round(neighborPos.x),
            Mathf.Round(neighborPos.y),
            Mathf.Round(neighborPos.z)
        );
        if (tiles.TryGetValue(neighborPos, out Tile neighborTile)
            && neighborTile.SpriteData != null && neighborTile.SpriteData.type == SpriteType.Block)
            return true;
        return false;
    }

    public Tile GetTileAtPosition(Vector3 position)
    {
        tiles.TryGetValue(position, out Tile tile);
        return tile;
    }

    public Rail GetRailAtPosition(Vector3 position)
    {
        return rails.FirstOrDefault(r => r.transform.position.x == position.x && r.transform.position.y == position.y);
    }

    public void SetActiveTraps(Trap trap)
    {
        if (trap == null)
        {
            clearActiveTraps();
            settings.UpdateSpikeSettingsView();
            settings.UpdateSawSettingsView();
            settings.UpdateCanonSettingsView();
            settings.UpdateAxeSettingsView();
            settings.UpdateBladeSettingsView();
            return;
        }

        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (shift)
        {
            if (activeTraps.Count == 0 || trap.trapType == activeTraps.First().trapType)
            {
                if (activeTraps.Add(trap))
                {
                    if (trap.trapType == TrapType.Spike) settings.UpdateSpikeSettingsView();
                    if (trap.trapType == TrapType.Saw) settings.UpdateSawSettingsView();
                    if (trap.trapType == TrapType.Canon) settings.UpdateCanonSettingsView();
                    if (trap.trapType == TrapType.Axe) settings.UpdateAxeSettingsView();
                    if (trap.trapType == TrapType.Blade) settings.UpdateBladeSettingsView();
                    trap.isActive = true;
                    trap.SetBorder();
                }
            }
            lastSelectedSprite[View.Traps] = trap.SpriteData;
            return;
        }
        clearActiveTraps();
        activeTraps.Add(trap);
        trap.isActive = true;
        trap.SetBorder();
        if (trap.trapType == TrapType.Spike) settings.UpdateSpikeSettingsView();
        if (trap.trapType == TrapType.Saw) settings.UpdateSawSettingsView();
        if (trap.trapType == TrapType.Canon) settings.UpdateCanonSettingsView();
        if (trap.trapType == TrapType.Axe) settings.UpdateAxeSettingsView();
        if (trap.trapType == TrapType.Blade) settings.UpdateBladeSettingsView();
        lastSelectedSprite[View.Traps] = trap.SpriteData;
    }

    private void clearActiveTraps()
    {
        foreach (var trap in activeTraps)
        {
            trap.isActive = false;
            trap.SetBorder();
        }
        activeTraps.Clear();
    }

    public HashSet<Trap> GetActiveTraps() => activeTraps;

    public void PaintRail(Vector3 position)
    {
        if (rails.Any(r => r.transform.position == position))
            return;

        Transform parent = gridParent.Find($"Layer {position.z}");
        Rail newRail = Instantiate(railPrefab, position, Quaternion.identity, parent);
        newRail.name = $"Rail {position.x} {position.y} {position.z}";
        newRail.TileRenderer.sortingOrder = 1;
        newRail.Init(this, position);
        rails.Add(newRail);

        UpdateRailAndNeighbors(newRail);
    }
    public void UpdateRailAndNeighbors(Rail rail)
    {
        Vector3 pos = rail.transform.position;

        UpdateRailSpriteAtPosition(pos, rail);

        UpdateRailSpriteAtPosition(pos + Vector3.up, rail);
        UpdateRailSpriteAtPosition(pos + Vector3.down, rail);
        UpdateRailSpriteAtPosition(pos + Vector3.left, rail);
        UpdateRailSpriteAtPosition(pos + Vector3.right, rail);

        LockRails(pos, true);
    }

    public void destroyRail(Rail rail)
    {
        if (rails.Contains(rail))
        {
            LockRails(rail.transform.position, false);
            Vector3 pos = rail.transform.position;
            Rail top = GetRailAtPosition(pos + Vector3.up);
            Rail bottom = GetRailAtPosition(pos + Vector3.down);
            Rail left = GetRailAtPosition(pos + Vector3.left);
            Rail right = GetRailAtPosition(pos + Vector3.right);
            rails.Remove(rail);

            List<Rail> neighbours = new List<Rail> { top, bottom, left, right };

            neighbours.ForEach(r =>
            {
                if (r == null) return;
                RailBitmapType type = DetermineRailType(GetRailAtPosition(r.transform.position + Vector3.up), GetRailAtPosition(r.transform.position + Vector3.down), GetRailAtPosition(r.transform.position + Vector3.left), GetRailAtPosition(r.transform.position + Vector3.right));
                r.SetSprite(type);
                LockRails(r.transform.position, true);
            });
            DestroyImmediate(rail.gameObject);
        }
    }

    public void destroyTrap(Trap trap)
    {
        if (allTraps.Contains(trap))
        {
            allTraps.Remove(trap);
            activeTraps.Remove(trap);
            if (trap is Saw saw)
            {
                saw.ResetToSpawn();
            }
            Vector3 roundedPosition = new Vector3(
                Mathf.Round(trap.transform.position.x),
                Mathf.Round(trap.transform.position.y),
                Mathf.Round(trap.transform.position.z)
            );
            ReplaceToTile(null, roundedPosition);
        }
    }
    private void LockRails(Vector3 position, bool locking)
    {
        Rail rail = GetRailAtPosition(position);
        if (rail == null) return;

        Rail top = GetRailAtPosition(position + Vector3.up);
        Rail bottom = GetRailAtPosition(position + Vector3.down);
        Rail left = GetRailAtPosition(position + Vector3.left);
        Rail right = GetRailAtPosition(position + Vector3.right);

        List<Rail> rails = new List<Rail> { rail, top, bottom, left, right };

        rails.ForEach(r =>
        {
            if (r == null) return;
            // Only lock lines and corners, NOT end pieces (ToEnd) or Center
            if (r.currentType == RailBitmapType.BottomToTop || r.currentType == RailBitmapType.LeftToRight || r.currentType == RailBitmapType.BottomToRight || r.currentType == RailBitmapType.BottomToLeft || r.currentType == RailBitmapType.TopToRight || r.currentType == RailBitmapType.LeftToUp)
            {
                if (!locking)
                {
                    Rail topN = GetRailAtPosition(r.transform.position + Vector3.up);
                    Rail bottomN = GetRailAtPosition(r.transform.position + Vector3.down);
                    Rail leftN = GetRailAtPosition(r.transform.position + Vector3.left);
                    Rail rightN = GetRailAtPosition(r.transform.position + Vector3.right);
                    List<Rail> neighborRails = new List<Rail> { topN, bottomN, leftN, rightN };
                    neighborRails.ForEach(nr =>
                    {
                        if (nr == null) return;
                        if (nr.currentType == RailBitmapType.BottomToTop || nr.currentType == RailBitmapType.LeftToRight || nr.currentType == RailBitmapType.BottomToRight || nr.currentType == RailBitmapType.BottomToLeft || nr.currentType == RailBitmapType.TopToRight || nr.currentType == RailBitmapType.LeftToUp)
                        { nr.isLocked = false; }
                    });
                    r.isLocked = false;
                }
                else
                {
                    r.isLocked = true;
                }
            }
        });
    }

    private void UpdateRailSpriteAtPosition(Vector3 position, Rail rail)
    {
        rail = GetRailAtPosition(position);
        if (rail == null || rail.isLocked) return;
        Rail top = GetRailAtPosition(position + Vector3.up);
        Rail bottom = GetRailAtPosition(position + Vector3.down);
        Rail left = GetRailAtPosition(position + Vector3.left);
        Rail right = GetRailAtPosition(position + Vector3.right);

        RailBitmapType type = DetermineRailType(top, bottom, left, right);

        rail.SetSprite(type);
    }

    private RailBitmapType DetermineRailType(Rail top, Rail bottom, Rail left, Rail right)
    {
        if (right == null && left == null && bottom == null && top == null)
        {
            return RailBitmapType.Center;
        }
        else if (right != null && bottom != null
        && (right.currentType == RailBitmapType.TopToEnd || right.currentType == RailBitmapType.BottomToEnd || right.currentType == RailBitmapType.LeftToRight || right.currentType == RailBitmapType.LeftToUp || right.currentType == RailBitmapType.BottomToLeft || right.currentType == RailBitmapType.LeftToEnd || right.currentType == RailBitmapType.RightToEnd || right.currentType == RailBitmapType.Center)
        && (bottom.currentType == RailBitmapType.LeftToEnd || bottom.currentType == RailBitmapType.RightToEnd || bottom.currentType == RailBitmapType.BottomToTop || bottom.currentType == RailBitmapType.TopToRight || bottom.currentType == RailBitmapType.LeftToUp || bottom.currentType == RailBitmapType.TopToEnd || bottom.currentType == RailBitmapType.BottomToEnd || bottom.currentType == RailBitmapType.Center))
        {
            return RailBitmapType.BottomToRight;
        }
        else if (left != null && bottom != null
        && (left.currentType == RailBitmapType.TopToEnd || left.currentType == RailBitmapType.BottomToEnd || left.currentType == RailBitmapType.LeftToRight || left.currentType == RailBitmapType.TopToRight || left.currentType == RailBitmapType.BottomToRight || left.currentType == RailBitmapType.LeftToEnd || left.currentType == RailBitmapType.RightToEnd || left.currentType == RailBitmapType.Center)
        && (bottom.currentType == RailBitmapType.LeftToEnd || bottom.currentType == RailBitmapType.RightToEnd || bottom.currentType == RailBitmapType.BottomToTop || bottom.currentType == RailBitmapType.TopToRight || bottom.currentType == RailBitmapType.LeftToUp || bottom.currentType == RailBitmapType.TopToEnd || bottom.currentType == RailBitmapType.BottomToEnd || bottom.currentType == RailBitmapType.Center))
        {
            return RailBitmapType.BottomToLeft;
        }
        else if (left != null && top != null
        && (left.currentType == RailBitmapType.TopToEnd || left.currentType == RailBitmapType.BottomToEnd || left.currentType == RailBitmapType.LeftToRight || left.currentType == RailBitmapType.TopToRight || left.currentType == RailBitmapType.BottomToRight || left.currentType == RailBitmapType.LeftToEnd || left.currentType == RailBitmapType.RightToEnd || left.currentType == RailBitmapType.Center)
        && (top.currentType == RailBitmapType.LeftToEnd || top.currentType == RailBitmapType.RightToEnd || top.currentType == RailBitmapType.BottomToTop || top.currentType == RailBitmapType.BottomToLeft || top.currentType == RailBitmapType.BottomToRight || top.currentType == RailBitmapType.TopToEnd || top.currentType == RailBitmapType.BottomToEnd || top.currentType == RailBitmapType.Center))
        {
            return RailBitmapType.LeftToUp;
        }
        else if (top != null && right != null
        && (top.currentType == RailBitmapType.LeftToEnd || top.currentType == RailBitmapType.RightToEnd || top.currentType == RailBitmapType.BottomToTop || top.currentType == RailBitmapType.BottomToRight || top.currentType == RailBitmapType.BottomToLeft || top.currentType == RailBitmapType.TopToEnd || top.currentType == RailBitmapType.BottomToEnd || top.currentType == RailBitmapType.Center)
        && (right.currentType == RailBitmapType.TopToEnd || right.currentType == RailBitmapType.BottomToEnd || right.currentType == RailBitmapType.LeftToRight || right.currentType == RailBitmapType.LeftToUp || right.currentType == RailBitmapType.BottomToLeft || right.currentType == RailBitmapType.LeftToEnd || right.currentType == RailBitmapType.RightToEnd || right.currentType == RailBitmapType.Center))
        {
            return RailBitmapType.TopToRight;
        }
        else if (top != null && bottom != null
        && (top.currentType == RailBitmapType.Center || top.currentType == RailBitmapType.BottomToEnd || top.currentType == RailBitmapType.TopToEnd || top.currentType == RailBitmapType.BottomToTop || top.currentType == RailBitmapType.BottomToRight || top.currentType == RailBitmapType.BottomToLeft || top.currentType == RailBitmapType.RightToEnd || top.currentType == RailBitmapType.LeftToEnd)
        && (bottom.currentType == RailBitmapType.Center || bottom.currentType == RailBitmapType.BottomToEnd || bottom.currentType == RailBitmapType.TopToEnd || bottom.currentType == RailBitmapType.BottomToTop || bottom.currentType == RailBitmapType.TopToRight || bottom.currentType == RailBitmapType.LeftToUp || bottom.currentType == RailBitmapType.RightToEnd || bottom.currentType == RailBitmapType.LeftToEnd))
        {
            return RailBitmapType.BottomToTop;
        }
        else if (left != null && right != null
        && (left.currentType == RailBitmapType.Center || left.currentType == RailBitmapType.LeftToEnd || left.currentType == RailBitmapType.RightToEnd || left.currentType == RailBitmapType.LeftToRight || left.currentType == RailBitmapType.TopToRight || left.currentType == RailBitmapType.BottomToRight || left.currentType == RailBitmapType.TopToEnd || left.currentType == RailBitmapType.BottomToEnd)
        && (right.currentType == RailBitmapType.Center || right.currentType == RailBitmapType.LeftToEnd || right.currentType == RailBitmapType.RightToEnd || right.currentType == RailBitmapType.LeftToRight || right.currentType == RailBitmapType.LeftToUp || right.currentType == RailBitmapType.BottomToLeft || right.currentType == RailBitmapType.TopToEnd || right.currentType == RailBitmapType.BottomToEnd))
        {
            return RailBitmapType.LeftToRight;
        }
        else if (top != null && !top.isLocked)
        {
            return RailBitmapType.TopToEnd;
        }
        else if (bottom != null && !bottom.isLocked)
        {
            return RailBitmapType.BottomToEnd;
        }
        else if (left != null && !left.isLocked)
        {
            return RailBitmapType.LeftToEnd;
        }
        else if (right != null && !right.isLocked)
        {
            return RailBitmapType.RightToEnd;
        }
        else
        {
            return RailBitmapType.Center;
        }
    }

    public bool IsSpawnSet()
    {
        return tiles.Any(t => t.Value.name == "Spawn");
    }

    public bool IsFinishSet()
    {
        return tiles.Any(t => t.Value.name == "Finish");
    }
    public void CreateMap(string mapName = "Map")
    {
        string sky = uiHandler.sky.GetComponent<Image>().sprite.name;
        Map map = new Map(tiles, rails, sky);
        string mapId = Guid.NewGuid().ToString();
        var jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        string json = JsonConvert.SerializeObject(map, Formatting.Indented, jsonSettings);

        API.MapData mapData = new API.MapData
        {
            map_id = mapId,
            created_by = UserManager.Instance.username,
            map_name = mapName,
            data = map
        };

        if (!File.Exists(Application.dataPath + "/Maps"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Maps");
        }
        if (!File.Exists(Application.dataPath + $"/Maps/{mapId}/"))
        {
            Directory.CreateDirectory(Application.dataPath + $"/Maps/{mapId}/");
        }
        string path = Application.dataPath + $"/Maps/{mapId}/{mapId}";
        File.WriteAllText(path + ".json", JsonConvert.SerializeObject(mapData, Formatting.Indented, jsonSettings));
        ScreenshotHandler.TakeScreenshot_Static(1920, 1080, path , () =>
        {
            api.CreateMap(mapId, UserManager.Instance.username, mapName, json, File.ReadAllBytes(path + ".png"));
        });
    }
}
