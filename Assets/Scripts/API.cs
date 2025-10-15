using System.Collections;
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
            ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);

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
            ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);

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

    [System.Serializable]
    public class User
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class ApiResponse
    {
        public string error;
        public string detail;
        public User userdata;
    }
}
