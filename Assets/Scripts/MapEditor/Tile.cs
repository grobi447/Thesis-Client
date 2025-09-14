using System.Collections.Generic;
using UnityEngine;

public enum SpriteType
{
    Block,
    Spike
}

public class SpriteData
{
    public string name;
    public Sprite sprite;
    public SpriteType type;
}

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileRenderer;
    [SerializeField] public Sprite emptySprite;

    private SpriteData spriteData;
    private GridManager gridManager;
    private UiHandler uiHandler;
    private Vector3 position;
    private bool canPlace = true;

    private Color visibleColor = new Color(1, 1, 1, 1);
    private Color invisibleColor = new Color(1, 1, 1, 0);
    public SpriteRenderer TileRenderer
    {
        get => tileRenderer;
        set => tileRenderer = value;
    }
    public SpriteData SpriteData
    {
        get => spriteData;
        set => spriteData = value;
    }

    public void Init(GridManager gridManager, Vector3 position)
    {
        this.gridManager = gridManager;
        uiHandler = GameObject.Find("UiHandler").GetComponent<UiHandler>();
        this.position = position;
    }

    public void OnMouseEnter()
    {
        HandleHover();
    }

    public void OnMouseExit()
    {
        tileRenderer.color = spriteData == null ? invisibleColor : visibleColor;
        canPlace = true;
    }

    public void OnMouseOver()
    {
        View currentView = uiHandler.GetCurrentView();
        if (canPlace && GridManager.isMouseDown && GridManager.hasSelectedSprite && currentView != View.Sky)
        {
            SpriteData selectedData = gridManager.GetLastSelectedSprite(currentView);

            if (uiHandler.GetCurrentTool() != Tool.Rubber)
            {
                if (selectedData.type != SpriteType.Block && this.GetType() == typeof(Tile))
                {
                    gridManager.ReplaceTileWithTrap(selectedData, position);
                    return;
                }
                if (selectedData.type == SpriteType.Block && this.GetType() != typeof(Tile))
                {
                    gridManager.ReplaceTrapWithTile(selectedData, position);
                    return;
                }
                spriteData = selectedData;
                tileRenderer.sprite = spriteData.sprite;
                gameObject.name = spriteData.name;
                gameObject.tag = spriteData.type.ToString();
            }

            UseTool();
        }
    }

    public void UseTool()
    {
        switch (uiHandler.GetCurrentTool())
        {
            case Tool.Brush:
                if (gridManager.GetCurrentLayer() == 0) visibleColor = new Color(1, 1, 1, 0.5f);
                else visibleColor = new Color(1, 1, 1, 1f);
                tileRenderer.color = visibleColor;
                break;
            case Tool.Rubber:
                spriteData = null;
                tileRenderer.sprite = emptySprite;
                gameObject.name = $"Tile {position.x} {position.y} {position.z}";
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }

    public void TurnOffCollider()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public void TurnOnCollider()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void HandleHover()
    {
        SpriteData selectedTile = gridManager.GetLastSelectedSprite(uiHandler.GetCurrentView());
        if (selectedTile != null && selectedTile.type == SpriteType.Spike && uiHandler.GetCurrentTool() == Tool.Brush)
        {
            switch (selectedTile.name)
            {
                case "SpikeTop":
                    if (!gridManager.HasBlockNeighbor(this, Vector3.down)) canPlace = false;
                    break;
                case "SpikeBottom":
                    if (!gridManager.HasBlockNeighbor(this, Vector3.up)) canPlace = false;
                    break;
                case "SpikeLeft":
                    if (!gridManager.HasBlockNeighbor(this, Vector3.right)) canPlace = false;
                    break;
                case "SpikeRight":
                    if (!gridManager.HasBlockNeighbor(this, Vector3.left)) canPlace = false;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            if (!canPlace) tileRenderer.color = new Color(1, 0, 0, 0.7f);
            else tileRenderer.color = new Color(0, 1, 0, 0.7f);
            return;
        }
        tileRenderer.color = new Color(1, 1, 1, 0.7f);
    }
}