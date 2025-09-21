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
    [SerializeField] private UiHandler uiHandler;

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
    }

    public void UpdateSpikeSettingsView()
    {
        var spikes = gridManager.GetActiveTraps()
            .Where(t => t.trapType == TrapType.Spike)
            .Cast<Spike>()
            .ToList();

        if (spikes.Count == 0)
        {
            spikeSettings.SetActive(false);
            sawSettings.SetActive(false);
            return;
        }

        spikeSettings.SetActive(true);
        sawSettings.SetActive(false);

        for (int i = 0; i < spikeSliders.Count; i++)
        {
            Slider slider = spikeSliders[i].GetComponentInChildren<Slider>();
            TextMeshProUGUI label = spikeSliders[i].GetComponentInChildren<TextMeshProUGUI>();
            slider.value = spikes[0].settings[keys[i]];
            label.text = labels[i] + slider.value.ToString("0.00") + " sec";
        }
    }

    public void UpdataSawSettingsView()
    {
        if(gridManager.GetLastSelectedSprite(uiHandler.GetCurrentView()) == null || 
           gridManager.GetLastSelectedSprite(uiHandler.GetCurrentView()).trapType != TrapType.Saw)
        {
            sawSettings.SetActive(false);
            return;
        }
        sawSettings.SetActive(true);
        spikeSettings.SetActive(false);
    }
}