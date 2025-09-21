using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.CompilerServices;
using System;

public class GridManager : MonoBehaviour
{
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
        GameObject parentObj = new GameObject("Tiles");
        gridParent = parentObj.transform;
        GenerateGrid();
    }

    private void Update()
    {
        isMouseDown = Input.GetMouseButton(0);
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
                trapTile.GetComponent<SpriteRenderer>().sprite = trapData.sprite;
                trapTile.emptySprite = trapPrefab.emptySprite;
                tiles[position] = trapTile;
                allTraps.Add((Trap)trapTile);
                trapTile.UseTool();
            }
        }
    }

    public void ReplaceToTile(SpriteData blockData, Vector3 position)
    {
        if (blockData == null) blockData = new SpriteData { name = "Empty", sprite = tilePrefab.emptySprite, type = SpriteType.Block };
        if (tiles.TryGetValue(position, out Tile oldTile))
        {
            Transform parent = oldTile.transform.parent;
            DestroyImmediate(oldTile.gameObject);

            Tile newTile = Instantiate(tilePrefab, position, Quaternion.identity, parent);
            newTile.name = blockData.name;
            newTile.Init(this, position);
            newTile.TileRenderer.sortingOrder = (int)position.z;
            newTile.SpriteData = blockData;
            newTile.GetComponent<SpriteRenderer>().sprite = blockData.sprite;
            newTile.emptySprite = tilePrefab.emptySprite;
            tiles[position] = newTile;
            if (oldTile is Trap) allTraps.Remove((Trap)oldTile);
            if (oldTile is Rail)
            {
                rails.Remove((Rail)oldTile);
            }
            newTile.UseTool();
        }
    }
    public void destroyRail(Rail rail)
    {
        if (rails.Contains(rail))
        {
            rails.Remove(rail);
            DestroyImmediate(rail.gameObject);
        }
    }
    public bool HasBlockNeighbor(Tile tile, Vector3 direction)
    {
        Vector3 neighborPos = tile.transform.position + direction;
        if (tiles.TryGetValue(neighborPos, out Tile neighborTile)
            && neighborTile.SpriteData != null && neighborTile.SpriteData.type == SpriteType.Block)
            return true;
        return false;
    }

    public Rail GetRailAtPosition(Vector3 position)
    {
        return rails.FirstOrDefault(r => r.transform.position == position);
    }

    public void SetActiveTraps(Trap trap)
    {
        if (trap == null)
        {
            clearActiveTraps();
            return;
        }

        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (shift)
        {
            if (activeTraps.Count == 0 || trap.trapType == activeTraps.First().trapType)
            {
                if (activeTraps.Add(trap))
                {
                    settings.UpdateSpikeSettingsView();
                    trap.isActive = true;
                    trap.SetBorder();
                }
            }
            return;
        }
        clearActiveTraps();
        activeTraps.Add(trap);
        trap.isActive = true;
        trap.SetBorder();
        settings.UpdateSpikeSettingsView();
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

        Transform parent = gridParent.GetChild((int)position.z);
        Rail newRail = Instantiate(railPrefab, position, Quaternion.identity, parent);
        newRail.name = $"Rail {position.x} {position.y} {position.z}";
        newRail.TileRenderer.sortingOrder = 2;
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
    }

    private void UpdateRailSpriteAtPosition(Vector3 position, Rail rail)
    {
        rail = GetRailAtPosition(position);
        if (rail == null) return;

        Rail top = GetRailAtPosition(position + Vector3.up);
        Rail bottom = GetRailAtPosition(position + Vector3.down);
        Rail left = GetRailAtPosition(position + Vector3.left);
        Rail right = GetRailAtPosition(position + Vector3.right);

        RailBitmapType type = DetermineRailType(top, bottom, left, right);

        rail.SetSprite(type);

        if (type == RailBitmapType.BottomToTop || type == RailBitmapType.LeftToRight || type == RailBitmapType.BottomToRight || type == RailBitmapType.BottomToLeft || type == RailBitmapType.TopToRight || type == RailBitmapType.LeftToUp)
        {
            rail.isLocked = true;
        }
    }

    private RailBitmapType DetermineRailType(Rail top, Rail bottom, Rail left, Rail right)
    {
        if (right != null && bottom != null && (right.currentType == RailBitmapType.LeftToEnd || right.currentType == RailBitmapType.LeftToRight || right.currentType == RailBitmapType.LeftToUp || right.currentType == RailBitmapType.BottomToLeft || right.currentType == RailBitmapType.RightToEnd || right.currentType == RailBitmapType.BottomToEnd)
            && (bottom.currentType == RailBitmapType.TopToEnd || bottom.currentType == RailBitmapType.BottomToTop || bottom.currentType == RailBitmapType.LeftToUp || bottom.currentType == RailBitmapType.TopToRight || bottom.currentType == RailBitmapType.BottomToEnd || bottom.currentType == RailBitmapType.LeftToEnd || bottom.currentType == RailBitmapType.RightToEnd))
        {
            return RailBitmapType.BottomToRight;
        }
        else if (left != null && bottom != null && (left.currentType == RailBitmapType.RightToEnd || left.currentType == RailBitmapType.LeftToRight || left.currentType == RailBitmapType.TopToRight || left.currentType == RailBitmapType.BottomToRight || left.currentType == RailBitmapType.LeftToEnd || left.currentType == RailBitmapType.BottomToEnd)
            && (bottom.currentType == RailBitmapType.TopToEnd || bottom.currentType == RailBitmapType.BottomToTop || bottom.currentType == RailBitmapType.TopToRight || bottom.currentType == RailBitmapType.LeftToUp || bottom.currentType == RailBitmapType.BottomToEnd || bottom.currentType == RailBitmapType.LeftToEnd || bottom.currentType == RailBitmapType.RightToEnd))
        {
            return RailBitmapType.BottomToLeft;
        }
        else if (top != null && right != null && (top.currentType == RailBitmapType.BottomToEnd || top.currentType == RailBitmapType.BottomToTop || top.currentType == RailBitmapType.BottomToLeft || top.currentType == RailBitmapType.BottomToRight || top.currentType == RailBitmapType.TopToEnd || top.currentType == RailBitmapType.LeftToEnd || top.currentType == RailBitmapType.RightToEnd)
            && (right.currentType == RailBitmapType.LeftToEnd || right.currentType == RailBitmapType.LeftToRight || right.currentType == RailBitmapType.LeftToUp || right.currentType == RailBitmapType.BottomToLeft || right.currentType == RailBitmapType.RightToEnd || right.currentType == RailBitmapType.TopToEnd || right.currentType == RailBitmapType.BottomToEnd))
        {
            return RailBitmapType.TopToRight;
        }
        else if (left != null && top != null && (left.currentType == RailBitmapType.RightToEnd || left.currentType == RailBitmapType.LeftToRight || left.currentType == RailBitmapType.TopToRight || left.currentType == RailBitmapType.BottomToRight || left.currentType == RailBitmapType.LeftToEnd || left.currentType == RailBitmapType.TopToEnd || left.currentType == RailBitmapType.BottomToEnd)
            && (top.currentType == RailBitmapType.BottomToEnd || top.currentType == RailBitmapType.BottomToTop || top.currentType == RailBitmapType.BottomToRight || top.currentType == RailBitmapType.BottomToLeft || top.currentType == RailBitmapType.TopToEnd || top.currentType == RailBitmapType.LeftToEnd || top.currentType == RailBitmapType.RightToEnd))
        {
            return RailBitmapType.LeftToUp;
        }
        else if (top != null && bottom != null && (top.currentType == RailBitmapType.BottomToEnd || top.currentType == RailBitmapType.BottomToLeft || top.currentType == RailBitmapType.BottomToRight || top.currentType == RailBitmapType.BottomToTop || top.currentType == RailBitmapType.LeftToEnd || top.currentType == RailBitmapType.RightToEnd || top.currentType == RailBitmapType.TopToEnd || top.currentType == RailBitmapType.Center)
            && (bottom.currentType == RailBitmapType.TopToEnd || bottom.currentType == RailBitmapType.LeftToUp || bottom.currentType == RailBitmapType.TopToRight || bottom.currentType == RailBitmapType.BottomToTop || bottom.currentType == RailBitmapType.LeftToEnd || bottom.currentType == RailBitmapType.RightToEnd || bottom.currentType == RailBitmapType.BottomToEnd || bottom.currentType == RailBitmapType.Center))
        {
            return RailBitmapType.BottomToTop;
        }
        else if (left != null && right != null && (left.currentType == RailBitmapType.RightToEnd || left.currentType == RailBitmapType.TopToRight || left.currentType == RailBitmapType.BottomToRight || left.currentType == RailBitmapType.LeftToRight || left.currentType == RailBitmapType.TopToEnd || left.currentType == RailBitmapType.BottomToEnd || left.currentType == RailBitmapType.LeftToEnd || left.currentType == RailBitmapType.Center)
            && (right.currentType == RailBitmapType.LeftToEnd || right.currentType == RailBitmapType.LeftToUp || right.currentType == RailBitmapType.BottomToLeft || right.currentType == RailBitmapType.LeftToRight || right.currentType == RailBitmapType.TopToEnd || right.currentType == RailBitmapType.BottomToEnd || right.currentType == RailBitmapType.RightToEnd || right.currentType == RailBitmapType.Center))
        {
            return RailBitmapType.LeftToRight;
        }
        else if (top != null && top.currentType != RailBitmapType.LeftToRight)
        {
            return RailBitmapType.TopToEnd;
        }
        else if (bottom != null && bottom.currentType != RailBitmapType.LeftToRight)
        {
            return RailBitmapType.BottomToEnd;
        }
        else if (left != null && left.currentType != RailBitmapType.BottomToTop)
        {
            return RailBitmapType.LeftToEnd;
        }
        else if (right != null && right.currentType != RailBitmapType.BottomToTop)
        {
            return RailBitmapType.RightToEnd;
        }
        else
        {
            return RailBitmapType.Center;
        }
    }
}
