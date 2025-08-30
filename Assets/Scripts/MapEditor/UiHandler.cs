using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum View
{
    Sky,
    Blocks
}

public class UiHandler : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private GameObject blocksView;
    [SerializeField] private GameObject skyView;
    [SerializeField] private GameObject sky;
    [SerializeField] private Sprite blueSky;
    [SerializeField] private Sprite greenSky;
    [SerializeField] private Sprite caveSky;

    private View currentView = View.Sky;

    void Start()
    {
        UpdateView();
    }

    public void OnRightButtonClick()
    {
        currentView++;
        if (currentView > View.Blocks)
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
            currentView = View.Blocks;
        }
        UpdateView();
    }

    public void UpdateView()
    {
        switch (currentView)
        {
            case View.Sky:
                label.text = "Sky";
                skyView.SetActive(true);
                blocksView.SetActive(false);
                break;
            case View.Blocks:
                label.text = "Blocks";
                skyView.SetActive(false);
                blocksView.SetActive(true);
                scrollbar.value = 1;
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
}   
