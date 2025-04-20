using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TextureManager : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private GridManager gridManager;
    SpriteData spriteData = new SpriteData();
    private Image imageComponent;
    private GameObject border;
    private static GameObject currentlySelectedBorder;

    void Awake() {
        Transform child = transform.Find("Image");
        Transform borderChild = transform.Find("Border");
        imageComponent = child.GetComponent<Image>();
        border = borderChild.gameObject;
        border.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData) {
        spriteData.name = gameObject.name;
        spriteData.sprite = imageComponent.sprite;
        gridManager.SetSelectedSprite(spriteData); 

        if (currentlySelectedBorder != null && currentlySelectedBorder != border) {
            currentlySelectedBorder.SetActive(false);
        }

        border.SetActive(true);

        currentlySelectedBorder = border;      
    }   
}
