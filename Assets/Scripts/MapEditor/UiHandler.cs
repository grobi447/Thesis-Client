using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum View
{
    Sky,
    Blocks,
    Traps
}

public enum Tool
{
    Brush,
    Rubber,
    Settings
}

public class UiHandler : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private GameObject blocksView;
    [SerializeField] private GameObject skyView;
    [SerializeField] private GameObject trapView;
    [SerializeField] private GameObject sky;
    [SerializeField] private Sprite blueSky;
    [SerializeField] private Sprite greenSky;
    [SerializeField] private Sprite caveSky;
    [SerializeField] private ToggleGroup toolToggleGroup;
    [SerializeField] private GameObject settingsToggle;
    [SerializeField] private Toggle brushToggle;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject trapSettingsPanel;
    [SerializeField] private Settings settings;

    private View currentView = View.Sky;
    private Tool currentTool = Tool.Brush;

    void Start()
    {
        UpdateView();
    }

    public void OnRightButtonClick()
    {
        currentView++;
        if (currentView > View.Traps)
        {
            currentView = View.Sky;
        }
        UpdateView();
    }

    public void OnLeftButtonClick()
    {
        currentView--;
        if (currentView < View.Sky)
        {
            currentView = View.Traps;
        }
        UpdateView();
    }

    public void UpdateView()
    {
        gridManager.SetSelectedSprite(currentView, gridManager.GetLastSelectedSprite(currentView));
        switch (currentView)
        {
            case View.Sky:
                label.text = "Sky";
                skyView.SetActive(true);
                blocksView.SetActive(false);
                trapView.SetActive(false);
                trapSettingsPanel.SetActive(false);
                if (settingsToggle.GetComponent<Toggle>().isOn)
                {
                    settingsToggle.GetComponent<Toggle>().isOn = false;
                    brushToggle.isOn = true;
                    ToggleCurrentTool();
                }
                settingsToggle.SetActive(false);

                break;
            case View.Blocks:
                label.text = "Blocks";
                skyView.SetActive(false);
                trapView.SetActive(false);
                blocksView.SetActive(true);
                trapSettingsPanel.SetActive(false);
                if (settingsToggle.GetComponent<Toggle>().isOn)
                {
                    settingsToggle.GetComponent<Toggle>().isOn = false;
                    brushToggle.isOn = true;
                    ToggleCurrentTool();
                }
                settingsToggle.SetActive(false);
                scrollbar.value = 1;
                break;
            case View.Traps:
                label.text = "Traps";
                blocksView.SetActive(false);
                skyView.SetActive(false);
                trapView.SetActive(true);
                settingsToggle.SetActive(true);
                trapSettingsPanel.SetActive(true);
                scrollbar.value = 1;
                settings.UpdateTrapSettingsView();
                break;
        }
    }


    public void UpdateSky(Sprite newSky)
    {
        sky.GetComponent<Image>().sprite = newSky;
    }

    public View GetCurrentView()
    {
        return currentView;
    }

    public void ToggleCurrentTool()
    {
        Toggle activeToggle = toolToggleGroup.ActiveToggles().FirstOrDefault();
        if (activeToggle.name == "Brush")
        {
            currentTool = Tool.Brush;
        }
        else if (activeToggle.name == "Rubber")
        {
            currentTool = Tool.Rubber;
        }
        else if (activeToggle.name == "Settings")
        {
            currentTool = Tool.Settings;
        }
        if (currentTool != Tool.Settings)
        {
            gridManager.SetActiveTraps(null);
        }
    }

    public Tool GetCurrentTool()
    {
        return currentTool;
    }
}
