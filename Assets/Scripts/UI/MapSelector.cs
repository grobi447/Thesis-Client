using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Objects;

namespace UI
{
    public class MapSelector : MonoBehaviour
    {
        public List<string> installedMaps = new List<string>();
        public List<GameObject> mapItems = new List<GameObject>();
        public List<GameObject> LeaderboardLines = new List<GameObject>();
        public string activeMapId;
        public GameObject mapPrefab;
        public GameObject linePrefab;
        public API api;
        public GameObject contentPanel;
        public GameObject leaderboardContentPanel;

        public GameObject leaderboardPanel;
        public GameObject loadingPanel;
        public GameObject loadedPanel;
        public GameObject spinner;
        public Scrollbar scrollbar;
        private bool waitingForRemoteMaps;

        public void LoadMaps()
        {
            LoadExistingMaps();
            LoadRemoteMaps();
            MapManager.Instance.installedMaps = installedMaps;
        }

        private void LoadRemoteMaps()
        {
            if (api == null)
            {
                return;
            }

            if (api.mapLoader != null && api.mapLoader.Count > 0)
            {
                LoadNewMaps();
                MapManager.Instance.installedMaps = installedMaps;
            }
            else if (!waitingForRemoteMaps && gameObject.activeInHierarchy)
            {
                StartCoroutine(WaitForRemoteMaps());
            }
        }

        private IEnumerator WaitForRemoteMaps()
        {
            waitingForRemoteMaps = true;
            if (spinner != null)
            {
                spinner.SetActive(true);
            }
            yield return new WaitUntil(() => api != null && api.mapLoader != null && api.mapLoader.Count > 0);
            LoadNewMaps();
            MapManager.Instance.installedMaps = installedMaps;
            waitingForRemoteMaps = false;
            if (spinner != null)
            {
                spinner.SetActive(false);
            }
        }

        public void LoadExistingMaps()
        {
            string mapsPath = Path.Combine(Application.dataPath, "Maps");
            if (!Directory.Exists(mapsPath))
            {
                Directory.CreateDirectory(mapsPath);
                return;
            }

            foreach (string directory in Directory.GetDirectories(mapsPath))
            {
                string mapId = Path.GetFileName(directory);
                if (string.IsNullOrEmpty(mapId) || installedMaps.Contains(mapId))
                {
                    continue;
                }

                string dataPath = Path.Combine(directory, $"{mapId}.json");
                if (!File.Exists(dataPath))
                {
                    continue;
                }

                Dictionary<string, object> mapData = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(dataPath));
                CreateMapItem(mapId, mapData);
            }
        }

        public void LoadNewMaps()
        {
            foreach (Dictionary<string, object> mapData in api.mapLoader)
            {
                if (!installedMaps.Contains(mapData["map_id"].ToString()))
                {
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
                    CreateMapItem(mapId, mapData);
                }
            }
        }

        private void CreateMapItem(string mapId, Dictionary<string, object> mapData)
        {
            installedMaps.Add(mapId);
            GameObject mapItem = Instantiate(mapPrefab, contentPanel.transform);
            mapItem.GetComponent<MapItem>().Initialize(
                mapId,
                mapData["map_name"].ToString(),
                mapData["created_by"].ToString()
            );
            mapItem.name = mapId;
            mapItem.transform.SetParent(contentPanel.transform, false);
            SetupMapItem(mapItem, mapData);
        }

        public void SetupMapItem(GameObject mapItem, Dictionary<string, object> mapData)
        {
            UpdatePreviewImage(mapItem);
            mapItems.Add(mapItem);
            GameObject loaded = mapItem.transform.Find("Loaded").gameObject;
            TextMeshProUGUI mapNameText = loaded.transform.Find("title").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI creatorText = loaded.transform.Find("user").GetComponent<TextMeshProUGUI>();
            mapNameText.text = mapData["map_name"].ToString();
            creatorText.text = "By: " + mapData["created_by"].ToString();
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

        public void UpdateActiveMap(string mapId)
        {
            activeMapId = mapId;
            MapManager.Instance.activeMapId = activeMapId;
            foreach (GameObject mapItem in mapItems)
            {
                GameObject loaded = mapItem.transform.Find("Loaded").gameObject;
                GameObject activeIndicator = loaded.transform.Find("ActivatedBorder").gameObject;
                if (mapItem.name == mapId)
                {
                    activeIndicator.SetActive(true);
                }
                else
                {
                    activeIndicator.SetActive(false);
                }

            }
            leaderboardPanel.SetActive(true);
            StartCoroutine(UpdateLeaderboardAndDisplay());
        }

        private IEnumerator UpdateLeaderboardAndDisplay()
        {
            loadingPanel.SetActive(true);
            loadedPanel.SetActive(false);

            yield return StartCoroutine(api.GetLeaderboardRequest(activeMapId));

            SortAndDisplayLeaderboard();
        }

        public void SortAndDisplayLeaderboard()
        {
            foreach (GameObject line in LeaderboardLines)
            {
                Destroy(line);
            }
            LeaderboardLines.Clear();

            if (api.leaderboard != null)
            {
                API.Leaderboard[] sortedList = api.leaderboard
                    .Where(e => e.leaderboard_deaths != null)
                    .OrderBy(e => e.leaderboard_deaths)
                    .ToArray();
                foreach (API.Leaderboard entry in sortedList)
                {
                    GameObject line = Instantiate(linePrefab, leaderboardContentPanel.transform);
                    GameObject textObj = line.transform.Find("User").gameObject;
                    GameObject deathObj = line.transform.Find("Death").gameObject;
                    TextMeshProUGUI userText = textObj.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI deathText = deathObj.GetComponent<TextMeshProUGUI>();
                    userText.text = entry.user;
                    deathText.text = entry.leaderboard_deaths.ToString();
                    LeaderboardLines.Add(line);
                }
                loadingPanel.SetActive(false);
                loadedPanel.SetActive(true);
            }
        }

        public void OnPlay()
        {
            if (!string.IsNullOrEmpty(activeMapId))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("InGame");
            }
        }
    }
}
