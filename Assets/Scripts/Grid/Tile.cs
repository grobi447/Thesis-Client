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
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer tileRenderer;

    private Color startColor;
    
    public void Init(bool isOffset, GridManager gridManager) {
        this.gridManager = gridManager;
        tileRenderer.color = isOffset ? offsetColor : baseColor;
        startColor = tileRenderer.color;
    }

    public void OnMouseEnter() {
        if (spriteData == null) {
            Color highlightColor = tileRenderer.color;
            highlightColor.r += 0.5f;
            highlightColor.g += 0.5f;
            highlightColor.b += 0.5f;
            tileRenderer.color = highlightColor;
        }
    }
    public void OnMouseExit() {
       if (spriteData == null) {
            tileRenderer.color = startColor;
        }
    }
    public void OnMouseOver() {
        if (GridManager.isMouseDown && GridManager.hasSelectedSprite) {
            spriteData = gridManager.GetSelectedSprite();
            tileRenderer.sprite = spriteData.sprite;
            gameObject.name = spriteData.name;
            tileRenderer.color = new Color(1, 1, 1, 1);
        }
    }
}