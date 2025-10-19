using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Unity.VisualStudio.Editor;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
    public List<string> installedMaps = new List<string>();
    public GameObject mapPrefab;
    public API api;
    public GameObject contentPanel;

    public void LoadMaps()
    {
        LoadExistingMaps();
        LoadNewMaps();
    }   

    public void LoadExistingMaps()
    {
        string mapsPath = Application.dataPath + "/Maps/";
        if (Directory.Exists(mapsPath))
        {
            string[] mapDirectories = Directory.GetDirectories(mapsPath);
            foreach (string dir in mapDirectories)
            {
                string mapId = Path.GetFileName(dir);
                string dataPath = Path.Combine(dir, $"{mapId}.json");
                if (File.Exists(dataPath) && !installedMaps.Contains(mapId))
                {
                    installedMaps.Add(mapId);
                    GameObject mapItem = Instantiate(mapPrefab, contentPanel.transform);
                    mapItem.name = mapId;
                    mapItem.transform.SetParent(contentPanel.transform, false);
                    UpdatePreviewImage(mapItem);
                }
            }
        }
        else
        {
            Directory.CreateDirectory(mapsPath);
        }
        
    }

    public void LoadNewMaps()
    {
        foreach (Dictionary<string, object> mapData in api.mapLoader)
        {
            Debug.Log(mapData["map_id"].ToString());
            Debug.Log(installedMaps.Count);
            if (!installedMaps.Contains(mapData["map_id"].ToString()))
            {
                installedMaps.Add(mapData["map_id"].ToString());
                GameObject mapItem = Instantiate(mapPrefab, contentPanel.transform);
                mapItem.name =  mapData["map_id"].ToString();
                mapItem.transform.SetParent(contentPanel.transform, false);
                string mapsPath = Application.dataPath + "/Maps/";      
                string mapId = mapData["map_id"].ToString();
                string mapDir = Path.Combine(mapsPath, mapId);
                if (!Directory.Exists(mapDir))
                {
                    Directory.CreateDirectory(mapDir);
                }
                string dataPath = Path.Combine(mapDir, $"{mapId}.json");
                string jsonData = JsonConvert.SerializeObject(mapData, Formatting.Indented);
                File.WriteAllText(dataPath, jsonData);
                api.DownloadMapImage(mapId);
                UpdatePreviewImage(mapItem);
            }
        }
    }

    public void UpdatePreviewImage(GameObject mapItem)
    {
        GameObject loading = mapItem.transform.Find("Loading").gameObject;
        GameObject loaded = mapItem.transform.Find("Loaded").gameObject;
        GameObject imageObj = loaded.transform.Find("Picture").gameObject;
        UnityEngine.UI.Image loadedImage = imageObj.GetComponent<UnityEngine.UI.Image>();
        string mapId = mapItem.name;
        string imagePath = Path.Combine(Application.dataPath, "Maps", mapId, mapId + ".png");
        if (File.Exists(imagePath))
        {
            SetPreviewSprite(imagePath, loadedImage, loading, loaded);
        }
        else
        {
            loading.SetActive(true);
            loaded.SetActive(false);
            StartCoroutine(WaitForImageAndSet(imagePath, loadedImage, loading, loaded));
        }
    }

    private void SetPreviewSprite(string imagePath, UnityEngine.UI.Image target, GameObject loading, GameObject loaded)
    {
        try
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (texture.LoadImage(imageData))
            {
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                target.sprite = sprite;
                loading.SetActive(false);
                loaded.SetActive(true);
            }
            else
            {
                loading.SetActive(true);
                loaded.SetActive(false);
            }
        }
        catch (System.Exception)
        {
            loading.SetActive(true);
            loaded.SetActive(false);
            StartCoroutine(WaitForImageAndSet(imagePath, target, loading, loaded));
        }
    }

    private IEnumerator WaitForImageAndSet(string imagePath, UnityEngine.UI.Image target, GameObject loading, GameObject loaded, float timeoutSeconds = 10f, float pollInterval = 0.25f)
    {
        float start = Time.realtimeSinceStartup;
        while (!File.Exists(imagePath) && Time.realtimeSinceStartup - start < timeoutSeconds)
        {
            yield return new WaitForSeconds(pollInterval);
        }

        if (File.Exists(imagePath))
        {
            yield return new WaitForSeconds(0.05f);
            SetPreviewSprite(imagePath, target, loading, loaded);
        }
        else
        {
            loading.SetActive(true);
            loaded.SetActive(false);
        }
    }
}
