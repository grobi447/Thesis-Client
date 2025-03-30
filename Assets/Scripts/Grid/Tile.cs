using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer tileRenderer;
    private Color startColor;
    
    public void Init(bool isOffset) {
        tileRenderer.color = isOffset ? offsetColor : baseColor;
        startColor = tileRenderer.color;
    }

    public void OnMouseEnter() {
        Color highlightColor = tileRenderer.color;
        highlightColor.r += 0.5f;
        highlightColor.g += 0.5f;
        highlightColor.b += 0.5f;
        tileRenderer.color = highlightColor;
    }

    public void OnMouseExit() {
        tileRenderer.color = startColor;
    }
}