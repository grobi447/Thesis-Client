using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
    [SerializeField] private int width, height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform cam;
    private Dictionary<Vector2, Tile> tiles;
    [SerializeField] private SpriteData selectedSpriteData;
    [SerializeField] private Transform gridParent;
    public static bool isMouseDown = false;
    public static bool hasSelectedSprite = false;

    void Start() {
        GameObject parentObj = new GameObject("GridParent");
        gridParent = parentObj.transform;

        GenerateGrid();
    }

     void Update() {
        isMouseDown = Input.GetMouseButton(0);
    }

    void GenerateGrid() {
        tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){
                Tile newTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity, gridParent);
                newTile.name = $"Tile {x} {y}";

                bool isOffset = (x + y) % 2 == 1; 
                newTile.Init(isOffset, this);   

                tiles[new Vector2(x, y)] = newTile;
            }
        }
    }

    public Tile GetTileAtPosition(Vector2 position) {
        if (tiles.TryGetValue(position, out Tile tile)){
            return tile;
        }
        return null;
    }

    public void SetSelectedSprite(SpriteData data) {
        this.selectedSpriteData = data;
        hasSelectedSprite = true;
    }
    public SpriteData GetSelectedSprite() {
        return selectedSpriteData;
    }
}


