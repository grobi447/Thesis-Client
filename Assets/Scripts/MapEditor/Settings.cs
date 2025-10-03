using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] public GameObject spikeSettings;
    [SerializeField] public List<GameObject> spikeSliders;
    [SerializeField] public GameObject sawSettings;
    [SerializeField] public GameObject sawSpeedSlider;
    [SerializeField] private UiHandler uiHandler;
    [SerializeField] public GameObject canonPanel;
    [SerializeField] public GameObject selectedCanonPanel;
    [SerializeField] public GameObject canonFireRateSlider;
    [SerializeField] public GameObject canonProjectileSpeedSlider;
    [SerializeField] public GameObject axePanel;
    [SerializeField] public GameObject axePaintPanel;
    [SerializeField] public GameObject axeSettingsPanel;
     [SerializeField] public GameObject axeSpeedSlider;

    private readonly string[] keys = { "startTime", "onTime", "offTime" };
    private readonly string[] labels = { "Start Delay: ", "On Time: ", "Off Time: " };

    private void Start()
    {
        for (int i = 0; i < spikeSliders.Count; i++)
        {
            int idx = i;
            Slider s = spikeSliders[idx].GetComponentInChildren<Slider>();
            s.onValueChanged.AddListener(value =>
            {
                TextMeshProUGUI label = spikeSliders[idx].GetComponentInChildren<TextMeshProUGUI>();
                string[] keys = { "startTime", "onTime", "offTime" };
                string[] labels = { "Start Delay: ", "On Time: ", "Off Time: " };
                label.text = labels[idx] + value.ToString("0.00") + " sec";

                float startValue = spikeSliders[0].GetComponentInChildren<Slider>().value;
                float onValue = spikeSliders[1].GetComponentInChildren<Slider>().value;
                float offValue = spikeSliders[2].GetComponentInChildren<Slider>().value;

                var spikes = gridManager.GetActiveTraps()
                    .Where(t => t.trapType == TrapType.Spike)
                    .Cast<Spike>();

                foreach (var spike in spikes)
                {
                    spike.settings["startTime"] = startValue;
                    spike.settings["onTime"] = onValue;
                    spike.settings["offTime"] = offValue;
                    spike.RestartTimer();
                }
            });
        }

        Slider sawSpeed = sawSpeedSlider.GetComponentInChildren<Slider>();
        sawSpeed.onValueChanged.AddListener(value =>
        {
            TextMeshProUGUI label = sawSpeedSlider.GetComponentInChildren<TextMeshProUGUI>();
            label.text = "Speed: " + value.ToString("0.00");
            var saws = gridManager.GetActiveTraps()
                .Where(t => t.trapType == TrapType.Saw)
                .Cast<Saw>();

            foreach (var saw in saws)
            {
                saw.speed = value;
            }

        });


        Slider canonFireRate = canonFireRateSlider.GetComponentInChildren<Slider>();
        canonFireRate.onValueChanged.AddListener(value =>
        {
            TextMeshProUGUI label = canonFireRateSlider.GetComponentInChildren<TextMeshProUGUI>();
            label.text = "Reload: " + value.ToString("0.00") + " sec";
            var canons = gridManager.GetActiveTraps()
                .Where(t => t.trapType == TrapType.Canon)
                .Cast<Canon>();
            foreach (var canon in canons)
            {
                canon.fireRate = value;
            }
        });

        Slider canonProjectileSpeed = canonProjectileSpeedSlider.GetComponentInChildren<Slider>();
        canonProjectileSpeed.onValueChanged.AddListener(value =>
        {
            TextMeshProUGUI label = canonProjectileSpeedSlider.GetComponentInChildren<TextMeshProUGUI>();
            label.text = "Bullet Speed: " + value.ToString("0.00");
            var canons = gridManager.GetActiveTraps()
                .Where(t => t.trapType == TrapType.Canon)
                .Cast<Canon>();
            foreach (var canon in canons)
            {
                canon.projectileSpeed = value;
            }
        });

        Slider axeSpeed = axeSpeedSlider.GetComponentInChildren<Slider>();
        axeSpeed.onValueChanged.AddListener(value =>
        {
            TextMeshProUGUI label = axeSpeedSlider.GetComponentInChildren<TextMeshProUGUI>();
            label.text = "Speed: " + value.ToString("0.00");
            var axes = gridManager.GetActiveTraps()
                .Where(t => t.trapType == TrapType.Axe)
                .Cast<Axe>();
            foreach (var axe in axes)
            {
                axe.speed = value;
            }
        });
    }

    public void UpdateSpikeSettingsView()
    {
        if (gridManager.GetLastSelectedSprite(uiHandler.GetCurrentView()) == null ||
           gridManager.GetLastSelectedSprite(uiHandler.GetCurrentView()).trapType != TrapType.Spike)
        {
            spikeSettings.SetActive(false);
            return;
        }
        var spikes = gridManager.GetActiveTraps()
            .Where(t => t.trapType == TrapType.Spike)
            .Cast<Spike>()
            .ToList();

        if (spikes.Count == 0)
        {
            spikeSettings.SetActive(false);
            sawSettings.SetActive(false);
            canonPanel.SetActive(false);
            return;
        }

        spikeSettings.SetActive(true);
        sawSettings.SetActive(false);
        canonPanel.SetActive(false);
        axePanel.SetActive(false);
        for (int i = 0; i < spikeSliders.Count; i++)
        {
            Slider slider = spikeSliders[i].GetComponentInChildren<Slider>();
            TextMeshProUGUI label = spikeSliders[i].GetComponentInChildren<TextMeshProUGUI>();
            slider.value = spikes[0].settings[keys[i]];
            label.text = labels[i] + slider.value.ToString("0.00") + " sec";
        }
    }

    public void UpdateSawSettingsView()
    {
        if (gridManager.GetLastSelectedSprite(uiHandler.GetCurrentView()) == null ||
           gridManager.GetLastSelectedSprite(uiHandler.GetCurrentView()).trapType != TrapType.Saw)
        {
            sawSettings.SetActive(false);
            return;
        }
        sawSettings.SetActive(true);
        spikeSettings.SetActive(false);
        canonPanel.SetActive(false);
        axePanel.SetActive(false);
        if (gridManager.activeTraps.Any(t => t.trapType == TrapType.Saw))
        {
            sawSpeedSlider.SetActive(true);
            Slider slider = sawSpeedSlider.GetComponentInChildren<Slider>();
            TextMeshProUGUI label = sawSpeedSlider.GetComponentInChildren<TextMeshProUGUI>();
            var saw = gridManager.GetActiveTraps()
                .Where(t => t.trapType == TrapType.Saw)
                .Cast<Saw>()
                .First();
            slider.value = saw.speed;
            label.text = "Speed: " + slider.value.ToString("0.00");
        }
        else
        {
            sawSpeedSlider.SetActive(false);
        }
    }

    public void UpdateCanonSettingsView()
    {
        if (gridManager.GetLastSelectedSprite(uiHandler.GetCurrentView()) == null ||
           gridManager.GetLastSelectedSprite(uiHandler.GetCurrentView()).trapType != TrapType.Canon)
        {
            selectedCanonPanel.SetActive(false);
            return;
        }
        spikeSettings.SetActive(false);
        sawSettings.SetActive(false);
        canonPanel.SetActive(true);
        axePanel.SetActive(false);
        if (gridManager.activeTraps.Any(t => t.trapType == TrapType.Canon))
        {
            selectedCanonPanel.SetActive(true);
            Slider fireRateSlider = canonFireRateSlider.GetComponentInChildren<Slider>();
            TextMeshProUGUI fireRateLabel = canonFireRateSlider.GetComponentInChildren<TextMeshProUGUI>();
            Slider projectileSpeedSlider = canonProjectileSpeedSlider.GetComponentInChildren<Slider>();
            TextMeshProUGUI projectileSpeedLabel = canonProjectileSpeedSlider.GetComponentInChildren<TextMeshProUGUI>();
            var canon = gridManager.GetActiveTraps()
                .Where(t => t.trapType == TrapType.Canon)
                .Cast<Canon>()
                .First();
            fireRateSlider.value = canon.fireRate;
            fireRateLabel.text = "Reload: " + fireRateSlider.value.ToString("0.00") + " sec";
            projectileSpeedSlider.value = canon.projectileSpeed;
            projectileSpeedLabel.text = "Bullet Speed: " + projectileSpeedSlider.value.ToString("0.00");
            ToggleGroup toggleGroup = uiHandler.canonDirectionToggleGroup;
            foreach (var toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                if (toggle.name == canon.canonType.ToString())
                {
                    toggle.isOn = true;
                }
            }
        }
        else
        {
            selectedCanonPanel.SetActive(false);
        }
    }

    public void UpdateAxeSettingsView()
    {
        if (gridManager.GetLastSelectedSprite(uiHandler.GetCurrentView()) == null ||
           gridManager.GetLastSelectedSprite(uiHandler.GetCurrentView()).trapType != TrapType.Axe)
        {
            axePanel.SetActive(false);
            return;
        }

        spikeSettings.SetActive(false);
        sawSettings.SetActive(false);
        canonPanel.SetActive(false);
        axePanel.SetActive(true);
        axePaintPanel.SetActive(true);
        if (gridManager.activeTraps.Any(t => t.trapType == TrapType.Axe))
        {
            axeSettingsPanel.SetActive(true);
            Slider slider = axeSpeedSlider.GetComponentInChildren<Slider>();
            TextMeshProUGUI label = axeSpeedSlider.GetComponentInChildren<TextMeshProUGUI>();
            var axe = gridManager.GetActiveTraps()
                .Where(t => t.trapType == TrapType.Axe)
                .Cast<Axe>()
                .First();
            slider.value = axe.speed;
            label.text = "Speed: " + slider.value.ToString("0.00");
            

        }
        else
        {
            axeSettingsPanel.SetActive(false);
        }
        
    }
}
