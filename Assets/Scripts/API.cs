using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class API : MonoBehaviour
{
    private static string baseURL = "https://52.58.160.54/";
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private NotificationManager notificationManager;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    private string pendingSuccessMessage;
    public MapSelector mapSelector;
    public List<Dictionary<string, object>> mapLoader = new List<Dictionary<string, object>>();
    public Leaderboard[] leaderboard;
    private class AcceptAllCertificatesSignedWithASelfSignedCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    private void OnMainMenuLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.Equals(scene.name, mainMenuSceneName)) return;

        SceneManager.sceneLoaded -= OnMainMenuLoaded;

        if (menuManager == null)
        {
            menuManager = FindObjectOfType<MenuManager>();
        }
        menuManager?.LoggedIn();

        if (!string.IsNullOrEmpty(pendingSuccessMessage))
        {
            if (notificationManager == null)
            {
                notificationManager = FindObjectOfType<NotificationManager>();
            }
            notificationManager?.OnSuccessMessage(pendingSuccessMessage);
            pendingSuccessMessage = null;
        }
    }

    public IEnumerator RegisterRequest(string username, string password)
    {
        string url = $"{baseURL}register/";

        string json = JsonUtility.ToJson(new User { username = username, password = password });

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.certificateHandler = new AcceptAllCertificatesSignedWithASelfSignedCertificate();

            yield return request.SendWebRequest();
            UserResponse apiResponse = JsonUtility.FromJson<UserResponse>(request.downloadHandler.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                notificationManager.OnSuccessMessage(apiResponse?.detail);
                menuManager.LoggedIn();
            }
            else
            {
                notificationManager.OnErrorMessage(apiResponse?.detail);
            }
        }
    }
    public IEnumerator LoginRequest(string username, string password)
    {
        string url = $"{baseURL}login/";

        string json = JsonUtility.ToJson(new User { username = username, password = password });

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.certificateHandler = new AcceptAllCertificatesSignedWithASelfSignedCertificate();

            yield return request.SendWebRequest();
            UserResponse apiResponse = JsonUtility.FromJson<UserResponse>(request.downloadHandler.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                notificationManager.OnSuccessMessage(apiResponse?.detail);
                UserManager.Instance.username = apiResponse.userdata.username;
                menuManager.LoggedIn();
            }
            else
            {
                notificationManager.OnErrorMessage(apiResponse?.detail);
            }
        }
    }
    public void Register(string username, string password)
    {
        StartCoroutine(RegisterRequest(username, password));
    }
    public void Login(string username, string password)
    {
        StartCoroutine(LoginRequest(username, password));
    }

    public void CreateMap(string mapId, string createdBy, string mapName, string data, byte[] image)
    {
        StartCoroutine(CreateMapRequest(mapId, createdBy, mapName, data, image));
    }

    public IEnumerator CreateMapRequest(string mapId, string createdBy, string mapName, string data, byte[] image)
    {
        string url = $"{baseURL}map-creator/";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("map_id", mapId),
                new MultipartFormDataSection("created_by", createdBy),
                new MultipartFormDataSection("map_name", mapName),
                new MultipartFormDataSection("data", data)
            };

            if (image != null && image.Length > 0)
            {
                formData.Add(new MultipartFormFileSection("image", image, "map_image.png", "image/png"));
            }

            byte[] boundary = UnityWebRequest.GenerateBoundary();
            request.uploadHandler = new UploadHandlerRaw(UnityWebRequest.SerializeFormSections(formData, boundary));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "multipart/form-data; boundary=" + System.Text.Encoding.UTF8.GetString(boundary));
            request.certificateHandler = new AcceptAllCertificatesSignedWithASelfSignedCertificate();

            yield return request.SendWebRequest();

            MapResponse apiResponse = JsonUtility.FromJson<MapResponse>(request.downloadHandler.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                pendingSuccessMessage = apiResponse.detail;
                SceneManager.sceneLoaded += OnMainMenuLoaded;
                SceneManager.LoadScene(mainMenuSceneName);
            }
            else
            {
                notificationManager.OnErrorMessage(apiResponse?.detail);
            }
        }
    }

    public void GetMaps()
    {
        StartCoroutine(GetMapsRequest());
    }

    public IEnumerator GetMapsRequest()
    {
        string url = $"{baseURL}maps/";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.certificateHandler = new AcceptAllCertificatesSignedWithASelfSignedCertificate();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                MapsResponse response = JsonUtility.FromJson<MapsResponse>(request.downloadHandler.text);
                List<Dictionary<string, object>> maps = new List<Dictionary<string, object>>();
                foreach (MapData mapData in response.maps)
                {
                    Dictionary<string, object> mapDict = new Dictionary<string, object>
                    {
                        ["map_id"] = mapData.map_id,
                        ["created_by"] = mapData.created_by,
                        ["map_name"] = mapData.map_name,
                        ["data"] = mapData.data
                    };
                    maps.Add(mapDict);
                }
                mapLoader = maps;
            }
        }
    }

    public void DownloadMapImage(string mapId)
    {
        StartCoroutine(DownloadMapImageRequest(mapId));
    }

    private IEnumerator DownloadMapImageRequest(string mapId)
    {
        string url = $"{baseURL}download/{mapId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.certificateHandler = new AcceptAllCertificatesSignedWithASelfSignedCertificate();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string imagePath = Application.dataPath + $"/Maps/{mapId}/{mapId}.png";
                File.WriteAllBytes(imagePath, request.downloadHandler.data);
            }
        }
    }

    public void GetLeaderboard(string mapId)
    {
        StartCoroutine(GetLeaderboardRequest(mapId));
    }

    public IEnumerator GetLeaderboardRequest(string mapId)
    {
        string url = $"{baseURL}leaderboard/{mapId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.certificateHandler = new AcceptAllCertificatesSignedWithASelfSignedCertificate();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                LeaderboardResponse response = JsonConvert.DeserializeObject<LeaderboardResponse>(request.downloadHandler.text);
                leaderboard = response?.leaderboard;
            }

        }
    }

    [System.Serializable]
    public class User
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class UserResponse
    {
        public string error;
        public string detail;
        public User userdata;
    }
    [System.Serializable]
    public class MapResponse
    {
        public string error;
        public string detail;
        public SingleMap mapdata;
    }
    [System.Serializable]
    public class SingleMap
    {
        public string mapId;
        public string createdBy;
        public string mapName;
        public string data;
    }

    [System.Serializable]
    public class MapsResponse
    {
        public MapData[] maps;
    }
    [System.Serializable]
    public class MapData
    {
        public string map_id;
        public string created_by;
        public string map_name;
        public Map data;
    }

    [System.Serializable]
    public class Leaderboard
    {
        public string map_id;
        public string user;
        public int? current_deaths;
        public int? leaderboard_deaths;
    }

    public class LeaderboardResponse
    {
        public Leaderboard[] leaderboard;
    }
}