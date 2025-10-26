using System;
using System.Collections.Generic;
using UnityEngine;


public enum SpriteType
{
    Empty,
    Block,
    Trap,
    Rail,
    Spawn,
    Finish
}

[Serializable]
public class SpriteData
{
    public string name;
    public Sprite sprite;
    public SpriteType type;
    public TrapType trapType;
}

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileRenderer;
    [SerializeField] public Sprite emptySprite;

    private SpriteData spriteData;
    public GridManager gridManager;
    public UiHandler uiHandler;
    public Vector3 position;
    public bool canPlace = true;

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

            if (uiHandler.GetCurrentTool() == Tool.Brush && selectedData != null)
            {
                if (selectedData.type != SpriteType.Block && selectedData.type != SpriteType.Spawn && selectedData.type != SpriteType.Finish && this.GetType() == typeof(Tile))
                {
                    gridManager.ReplaceToTrap(selectedData, position);
                    return;
                }
                if ((selectedData.type == SpriteType.Block || selectedData.type == SpriteType.Spawn || selectedData.type == SpriteType.Finish) && this.GetType() != typeof(Tile) && this.GetType() != typeof(Rail))
                {
                    gridManager.ReplaceToTile(selectedData, position);
                    return;
                }
                if (selectedData.type == SpriteType.Trap && this.GetType() != typeof(Tile))
                {
                    gridManager.ReplaceToTrap(selectedData, position);
                    return;
                }
                spriteData = selectedData;
                tileRenderer.sprite = spriteData.sprite;
                gameObject.name = spriteData.name;
                gameObject.tag = spriteData.type.ToString();
            }

            if (uiHandler.GetCurrentTool() == Tool.Rubber)
            {
                if (this is Rail)
                {
                    gridManager.destroyRail((Rail)this);
                    return;
                }
                if (this is Trap)
                {
                    gridManager.destroyTrap((Trap)this);
                    return;
                }
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
                Rail rail = gridManager.GetRailAtPosition(this.transform.position);
                if (rail != null)
                {
                    rail.GetComponent<Collider2D>().layerOverridePriority = 1;
                    rail.GetComponent<Collider2D>().enabled = false;
                    rail.GetComponent<Collider2D>().enabled = true;
                }
                break;
            case Tool.Settings:
                if (this.GetType().BaseType == typeof(Trap))
                {
                    gridManager.SetActiveTraps((Trap)this);
                }
                break;
            case Tool.Rail:
                gridManager.PaintRail(position);
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

        if (selectedTile != null)
        {
            if (selectedTile.type == SpriteType.Trap)
            {
                HandleTrapHover(selectedTile);
                return;
            }
            if (selectedTile.type == SpriteType.Spawn && uiHandler.GetCurrentTool() == Tool.Brush)
            {
                HandleSpawnHover();
                return;
            }
            if (selectedTile.type == SpriteType.Finish && uiHandler.GetCurrentTool() == Tool.Brush)
            {
                HandleFinishHover();
                return;
            }
            tileRenderer.color = new Color(1, 1, 1, 0.7f);

        }
    }

    public void HandleTrapHover(SpriteData selectedTile)
    {
        if (selectedTile.trapType == TrapType.Spike)
        {
            HandleSpikeHover(selectedTile);
        }
        if (selectedTile.trapType == TrapType.Saw)
        {
            if (uiHandler.GetCurrentTool() == Tool.Rail)
            {
                HandlerRailHover();
                return;
            }
            else if (uiHandler.GetCurrentTool() == Tool.Brush)
            {

                HandleSawHover();
                return;
            }
            tileRenderer.color = new Color(1, 1, 1, 0.7f);
        }
        if (selectedTile.trapType == TrapType.Canon)
        {
            if (uiHandler.GetCurrentTool() == Tool.Brush)
            {
                HandleCanonHover();
                return;
            }
            tileRenderer.color = new Color(1, 1, 1, 0.7f);
        }
        if (selectedTile.trapType == TrapType.Axe)
        {
            if (uiHandler.GetCurrentTool() == Tool.Brush)
            {
                HandleAxeHover();
                return;
            }
            tileRenderer.color = new Color(1, 1, 1, 0.7f);
        }
        if (selectedTile.trapType == TrapType.Blade)
        {
            if (uiHandler.GetCurrentTool() == Tool.Brush)
            {
                HandleBladeHover();
                return;
            }
            tileRenderer.color = new Color(1, 1, 1, 0.7f);
        }

    }

    public void HandleSpikeHover(SpriteData selectedTile)
    {
        if (uiHandler.GetCurrentTool() == Tool.Brush)
        {
            if (this.SpriteData != null) canPlace = false;
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
            if (!canPlace) TileRenderer.color = new Color(1, 0, 0, 0.7f);
            else TileRenderer.color = new Color(0, 1, 0, 0.7f);
            return;
        }

        if (uiHandler.GetCurrentTool() == Tool.Rubber)
        {
            TileRenderer.color = new Color(1, 1, 1, 0.7f);
            return;
        }

        if (uiHandler.GetCurrentTool() == Tool.Settings)
        {
            if (SpriteData != null && SpriteData.type != SpriteType.Block)
                TileRenderer.color = new Color(0, 1, 0, 0.7f);
            else
                TileRenderer.color = new Color(1, 0, 0, 0.7f);
        }
    }

    public void HandlerRailHover()
    {
        canPlace = this.GetType().BaseType != typeof(Trap) && this.SpriteData == null;
        if (!canPlace) TileRenderer.color = new Color(1, 0, 0, 0.7f);
        else TileRenderer.color = new Color(0, 1, 0, 0.7f);
    }

    public void HandleSawHover()
    {
        canPlace = this.GetType() == typeof(Rail);
        if (!canPlace) TileRenderer.color = new Color(1, 0, 0, 0.7f);
        else TileRenderer.color = new Color(0, 1, 0, 0.7f);
    }

    public void HandleCanonHover()
    {
        canPlace = this.GetType() == typeof(Tile) && this.SpriteData == null;
        if (!canPlace) TileRenderer.color = new Color(1, 0, 0, 0.7f);
        else TileRenderer.color = new Color(0, 1, 0, 0.7f);
    }

    public void HandleAxeHover()
    {
        tileRenderer.color = new Color(1, 1, 1, 0.7f);
    }
    public void HandleBladeHover()
    {
        tileRenderer.color = new Color(1, 1, 1, 0.7f);
    }
    public void HandleSpawnHover()
    {
        canPlace = this.SpriteData == null && !gridManager.IsSpawnSet() && gridManager.HasBlockNeighbor(this, Vector3.down);
        if (!canPlace) TileRenderer.color = new Color(1, 0, 0, 0.7f);
        else TileRenderer.color = new Color(0, 1, 0, 0.7f);
    }
    public void HandleFinishHover()
    {
        canPlace = this.SpriteData == null && !gridManager.IsFinishSet() && gridManager.HasBlockNeighbor(this, Vector3.down);
        if (!canPlace) TileRenderer.color = new Color(1, 0, 0, 0.7f);
        else TileRenderer.color = new Color(0, 1, 0, 0.7f);
    }
}
