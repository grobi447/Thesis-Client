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
    Settings,
    Rail
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
    [SerializeField] public GameObject sky;
    [SerializeField] private Sprite blueSky;
    [SerializeField] private Sprite greenSky;
    [SerializeField] private Sprite caveSky;
    [SerializeField] private ToggleGroup toolToggleGroup;
    [SerializeField] private GameObject settingsToggle;
    [SerializeField] public Toggle brushToggle;
    [SerializeField] private Toggle railToggle;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject trapSettingsPanel;
    [SerializeField] private Settings settings;
    [SerializeField] public ToggleGroup canonDirectionToggleGroup;
    [SerializeField] public Toggle canonTargetPlayerToggle;
    [SerializeField] public ToggleGroup axeDirectionToggleGroup;
    [SerializeField] public ToggleGroup axeMovementToggleGroup;
    [SerializeField] private NotificationManager notificationManager;
    [SerializeField] private GameObject savePanel;
    [SerializeField] private TMPro.TMP_InputField mapNameInput;
    bool isSavePanelOpen = false;
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
        brushToggle.isOn = true;
        switch (currentView)
        {
            case View.Sky:
                label.text = "Sky";
                skyView.SetActive(true);
                blocksView.SetActive(false);
                trapView.SetActive(false);
                trapSettingsPanel.SetActive(false);
                settingsToggle.SetActive(false);
                savePanel.SetActive(false);
                isSavePanelOpen = false;
                break;
            case View.Blocks:
                label.text = "Blocks";
                skyView.SetActive(false);
                trapView.SetActive(false);
                blocksView.SetActive(true);
                trapSettingsPanel.SetActive(false);
                settingsToggle.SetActive(false);
                scrollbar.value = 1;
                savePanel.SetActive(false);
                isSavePanelOpen = false;
                break;
            case View.Traps:
                label.text = "Traps";
                blocksView.SetActive(false);
                skyView.SetActive(false);
                trapView.SetActive(true);
                settingsToggle.SetActive(true);
                trapSettingsPanel.SetActive(true);
                scrollbar.value = 1;
                settings.UpdateSpikeSettingsView();
                settings.UpdateSawSettingsView();
                settings.UpdateCanonSettingsView();
                settings.UpdateAxeSettingsView();
                savePanel.SetActive(false);
                isSavePanelOpen = false;
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
        else if (activeToggle.name == "Rail")
        {
            currentTool = Tool.Rail;
        }
        if (currentTool != Tool.Settings && currentTool != Tool.Rail)
        {
            gridManager.SetActiveTraps(null);
        }
        isSavePanelOpen = false;
        savePanel.SetActive(false);
    }

    public Tool GetCurrentTool()
    {
        return currentTool;
    }

    public void ToggleCanonDirection()
    {
        Toggle activeDirectionToggle = canonDirectionToggleGroup.ActiveToggles().FirstOrDefault();
        if (activeDirectionToggle.name == "Left")
        {
            gridManager.currentCanonDirection = CanonType.Left;
        }
        else if (activeDirectionToggle.name == "Right")
        {
            gridManager.currentCanonDirection = CanonType.Right;
        }
        else if (activeDirectionToggle.name == "Up")
        {
            gridManager.currentCanonDirection = CanonType.Up;
        }
        else if (activeDirectionToggle.name == "Down")
        {
            gridManager.currentCanonDirection = CanonType.Down;
        }

        List<Canon> activeCanons = gridManager.GetActiveTraps().Where(t => t is Canon).Select(t => (Canon)t).ToList();
        foreach (Canon canon in activeCanons)
        {
            canon.canonType = gridManager.currentCanonDirection;
            canon.UpdateCanon();
        }
    }

    public void ToggleCanonTargeting()
    {
        bool isTargeting = canonTargetPlayerToggle.isOn;
        List<Canon> activeCanons = gridManager.GetActiveTraps().Where(t => t is Canon).Select(t => (Canon)t).ToList();
        foreach (Canon canon in activeCanons)
        {
            canon.targetingPlayer = isTargeting;
            canon.ResetPosition();
        }
    }

    public void ToggleAxeDirection()
    {
        Toggle axeDirectionToggle = axeDirectionToggleGroup.ActiveToggles().FirstOrDefault();
        if (axeDirectionToggle.name == "Left")
        {
            gridManager.currentAxeDirection = AxeDirection.Left;
        }
        else if (axeDirectionToggle.name == "Right")
        {
            gridManager.currentAxeDirection = AxeDirection.Right;
        }
        else if (axeDirectionToggle.name == "Up")
        {
            gridManager.currentAxeDirection = AxeDirection.Up;
        }
        else if (axeDirectionToggle.name == "Down")
        {
            gridManager.currentAxeDirection = AxeDirection.Down;
        }

        List<Axe> activeAxes = gridManager.GetActiveTraps().Where(t => t is Axe).Select(t => (Axe)t).ToList();
        foreach (Axe axe in activeAxes)
        {
            axe.axeDirection = gridManager.currentAxeDirection;
            axe.SetDirection(axe.axeDirection);
        }
    }

    public void ToggleAxeMovement()
    {
        Toggle axeMovementToggle = axeMovementToggleGroup.ActiveToggles().FirstOrDefault();
        if (axeMovementToggle.name == "Half")
        {
            gridManager.currentAxeMovement = AxeMovement.Half;
        }
        else if (axeMovementToggle.name == "Circle")
        {
            gridManager.currentAxeMovement = AxeMovement.Circle;
        }

        List<Axe> activeAxes = gridManager.GetActiveTraps().Where(t => t is Axe).Select(t => (Axe)t).ToList();
        foreach (Axe axe in activeAxes)
        {
            axe.axeMovement = gridManager.currentAxeMovement;
        }
    }

    public void OnSaveButton()
    {
        if(!isSavePanelOpen)
        {
            savePanel.SetActive(true);
            isSavePanelOpen = true;
        }
        else
        {
        if(!gridManager.IsSpawnSet())
        {
            notificationManager.OnErrorMessage("Must have a spawn point!");
            return;
        }
        if(!gridManager.IsFinishSet())
        {
            notificationManager.OnErrorMessage("Must have an end point!");
            return;
        }

        if(string.IsNullOrWhiteSpace(mapNameInput.text))
        {
            notificationManager.OnErrorMessage("Map name is empty!");
            return;
        }
        //todo: check if map name already exists
        gridManager.CreateMap(mapNameInput.text);
        notificationManager.OnSuccessMessage("Map saved successfully!");
        }
    }
}