using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TextureManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GridManager gridManager;
    SpriteData spriteData = new SpriteData();
    private Image imageComponent;
    private GameObject border;
    private static Dictionary<View, GameObject> currentlySelectedBorder = new Dictionary<View, GameObject>
        {
            { View.Sky, null },
            { View.Blocks, null }
        };
    private UiHandler uiHandler;
    void Awake()
    {
        Transform child = transform.Find("Image");
        Transform borderChild = transform.Find("Border");
        imageComponent = child.GetComponent<Image>();
        border = borderChild.gameObject;
        uiHandler = GameObject.Find("UiHandler").GetComponent<UiHandler>();
        border.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        spriteData.name = gameObject.name;
        spriteData.sprite = imageComponent.sprite;
        switch (uiHandler.GetCurrentView())
        {
            case View.Sky:
                if (uiHandler.GetCurrentView() == View.Sky)
                {
                    uiHandler.UpdateSky(spriteData.sprite);
                }
                break;
            case View.Blocks:
                gridManager.SetSelectedSprite(spriteData);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }



        if (currentlySelectedBorder[uiHandler.GetCurrentView()] != null && currentlySelectedBorder[uiHandler.GetCurrentView()] != border)
        {
            currentlySelectedBorder[uiHandler.GetCurrentView()].SetActive(false);
        }

        border.SetActive(true);

        currentlySelectedBorder[uiHandler.GetCurrentView()] = border;

    }

    public void setCurrentlySelectedBorder(View view, GameObject border)
    {
        currentlySelectedBorder[view] = border;
    }
}
