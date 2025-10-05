using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TextureManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GridManager gridManager;
    private Settings settings;

    private Image imageComponent;
    private GameObject border;
    private UiHandler uiHandler;
    private SpriteData spriteData = new SpriteData();

    private static Dictionary<View, GameObject> currentlySelectedBorder = new Dictionary<View, GameObject>
        {
            { View.Sky, null },
            { View.Blocks, null },
            { View.Traps, null }
        };

    private void Awake()
    {
        Transform child = transform.Find("Image");
        Transform borderChild = transform.Find("Border");
        imageComponent = child.GetComponent<Image>();
        border = borderChild.gameObject;
        uiHandler = GameObject.Find("UiHandler").GetComponent<UiHandler>();
        settings = GameObject.FindAnyObjectByType<Settings>();
        border.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        spriteData.name = gameObject.name;
        spriteData.sprite = imageComponent.sprite;
        gridManager.SetActiveTraps(null);
        switch (uiHandler.GetCurrentView())
        {
            case View.Sky:
                uiHandler.UpdateSky(spriteData.sprite);
                break;
            case View.Blocks:
            case View.Traps:
                gridManager.SetSelectedSprite(uiHandler.GetCurrentView(), spriteData);
                spriteData.type = gameObject.tag switch
                {
                    "Block" => SpriteType.Block,
                    "Trap" => SpriteType.Trap,
                    "Spawn" => SpriteType.Spawn,
                    "Finish" => SpriteType.Finish,
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (spriteData.type == SpriteType.Trap)
                {
                    if (gameObject.name.Contains("Spike"))
                    {
                        spriteData.trapType = TrapType.Spike;
                        uiHandler.brushToggle.isOn = true;
                    }
                    else
                    if (gameObject.name.Contains("Saw"))
                    {
                        spriteData.trapType = TrapType.Saw;
                        settings.UpdateSawSettingsView();
                    }
                    if (gameObject.name.Contains("Canon"))
                    {
                        spriteData.trapType = TrapType.Canon;
                        settings.UpdateCanonSettingsView();
                    }
                    if (gameObject.name.Contains("Axe"))
                    {
                        spriteData.trapType = TrapType.Axe;
                        settings.UpdateAxeSettingsView();
                    }
                    if (gameObject.name.Contains("Blade"))
                    {
                        spriteData.trapType = TrapType.Blade;
                        settings.UpdateBladeSettingsView();
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }



        View currentView = uiHandler.GetCurrentView();
        if (currentlySelectedBorder[currentView] != null && currentlySelectedBorder[currentView] != border)
        {
            currentlySelectedBorder[currentView].SetActive(false);
        }

        border.SetActive(true);

        currentlySelectedBorder[currentView] = border;
    }

    public void SetCurrentlySelectedBorder(View view, GameObject border)
    {
        currentlySelectedBorder[view] = border;
    }
}
