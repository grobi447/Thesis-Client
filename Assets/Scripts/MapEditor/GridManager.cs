using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

    public static bool isMouseDown = false;
    public static bool hasSelectedSprite = false;
    public static int currentLayer = 1;

    private Dictionary<Vector3, Tile> tiles;
    private Dictionary<string, Tile> trapPrefabDict;

    public Tile TilePrefab => tilePrefab;
    public Dictionary<string, Tile> TrapPrefabDict => trapPrefabDict;

    [SerializeField]
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

    public SpriteData GetSelectedSprite() => selectedSpriteData;

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

    public void ReplaceTileWithTrap(SpriteData trapData, Vector3 position)
    {
        if (tiles.TryGetValue(position, out Tile oldTile))
        {
            Transform parent = oldTile.transform.parent;
            DestroyImmediate(oldTile.gameObject);

            if (trapPrefabDict.TryGetValue(trapData.name, out Tile trapPrefab))
            {
                Tile trapTile = Instantiate(trapPrefab, position, Quaternion.identity, parent);
                trapTile.name = trapData.name;
                trapTile.Init(this, position);
                trapTile.TileRenderer.sortingOrder = (int)position.z;
                trapTile.SpriteData = trapData;
                trapTile.GetComponent<SpriteRenderer>().sprite = trapData.sprite;
                trapTile.emptySprite = trapPrefab.emptySprite;
                tiles[position] = trapTile;
                trapTile.UseTool();
            }
        }
    }

    public void ReplaceTrapWithTile(SpriteData blockData, Vector3 position)
    {
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
            newTile.UseTool();
        }
    }
}


