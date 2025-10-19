using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class API : MonoBehaviour
{
    private static string baseURL = "https://52.58.160.54/";
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private NotificationManager notificationManager;
    private class AcceptAllCertificatesSignedWithASelfSignedCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
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
                notificationManager.OnSuccessMessage(apiResponse?.detail);
            }
            else
            {
                notificationManager.OnErrorMessage(apiResponse?.detail);
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
        public Map mapdata;
    }
    [System.Serializable]
    public class Map
    {
        public string mapId;
        public string createdBy;
        public string mapName;
        public string data;
    }
}
