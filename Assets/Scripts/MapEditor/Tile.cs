using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpriteData
{
    public string name;
    public Sprite sprite;
}

public class Tile : MonoBehaviour
{
    private SpriteData spriteData;
    private GridManager gridManager;
    [SerializeField] private SpriteRenderer tileRenderer;
    private Color baseColor = new Color(1, 1, 1, 1);
    private Color startColor;
    private UiHandler uiHandler;

    public void Init(GridManager gridManager)
    {
        this.gridManager = gridManager;
        startColor = tileRenderer.color;
        uiHandler = GameObject.Find("UiHandler").GetComponent<UiHandler>();
    }

    public void OnMouseEnter()
    {
        tileRenderer.color = new Color(1, 1, 1, 0.7f);
        
    }

    public void OnMouseExit()
    {
        if (spriteData == null)
        {
            tileRenderer.color = startColor;
        }
        else
        {
            tileRenderer.color = baseColor;
        }
    }
    
    public void OnMouseOver()
    {
        if (GridManager.isMouseDown && GridManager.hasSelectedSprite && uiHandler.GetCurrentView() != View.Sky)
        {
            spriteData = gridManager.GetSelectedSprite();
            tileRenderer.sprite = spriteData.sprite;
            gameObject.name = spriteData.name;
            tileRenderer.color = baseColor;
        }
    }
}