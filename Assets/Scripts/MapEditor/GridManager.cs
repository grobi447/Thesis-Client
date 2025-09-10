using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform cam;
    private Dictionary<Vector2, Tile> tiles;
    [SerializeField] private SpriteData selectedSpriteData;
    [SerializeField] private Transform gridParent;
    public static bool isMouseDown = false;
    public static bool hasSelectedSprite = false;
    [SerializeField] private ToggleGroup toggleGroup;
    public static int currentLayer = 1;

    void Start()
    {
        GameObject parentObj = new GameObject("Tiles");
        gridParent = parentObj.transform;

        GenerateGrid();
    }

    void Update()
    {
        isMouseDown = Input.GetMouseButton(0);
    }

    void GenerateGrid()
    {
        tiles = new Dictionary<Vector2, Tile>();
        for (int z = 0; z < 2; z++)
        {
            GameObject layer = new GameObject($"Layer {z}");
            layer.transform.parent = gridParent;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tile newTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity, layer.transform);
                    newTile.name = $"Tile {x} {y} {z}";

                    newTile.Init(this);

                    tiles[new Vector2(x, y)] = newTile;
                }
            }
        }
        UpdateColliders();
    }

    public void SetSelectedSprite(SpriteData data)
    {
        this.selectedSpriteData = data;
        hasSelectedSprite = true;
    }
    public SpriteData GetSelectedSprite()
    {
        return selectedSpriteData;
    }

    public void CheckActiveToggle()
    {
        Toggle activeToggle = toggleGroup.ActiveToggles().FirstOrDefault();

        if (activeToggle.name == "Background")
        {
            currentLayer = 0;
        }
        else if (activeToggle.name == "Playfield")
        {
            currentLayer = 1;
        }
        UpdateColliders();
        Debug.Log($"Current layer set to {currentLayer}");
    }

    public void UpdateColliders()
    {
        for (int z = 0; z < 2; z++)
        {
            Transform layer = gridParent.Find($"Layer {z}");
            foreach (Transform tile in layer)
            {
                if (z == currentLayer)
                {
                    tile.GetComponent<Tile>().TurnOnCollider();
                }
                else
                {
                    tile.GetComponent<Tile>().TurnOffCollider();
                }
            }
        }
    }
    public int GetCurrentLayer()
    {
        return currentLayer;
    }
}


