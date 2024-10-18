using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class API : MonoBehaviour
{
    private static string baseURL = "https://52.58.160.54/";
    public MenuManager menuManager;

    private class AcceptAllCertificatesSignedWithASelfSignedCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    public IEnumerator RegisterRequest(string endpoint, string username, string password)
    {
        string url = $"{baseURL}{endpoint}";

        string json = JsonUtility.ToJson(new UserRegister { username = username, password = password });

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.certificateHandler = new AcceptAllCertificatesSignedWithASelfSignedCertificate();

            yield return request.SendWebRequest();
            string responseText = request.downloadHandler.text;
            ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(responseText);

            if (request.result == UnityWebRequest.Result.Success)
            {
                menuManager.OnSuccessMessage(apiResponse.detail);
                menuManager.LoggedIn();
            }
            else
            {
                menuManager.OnErrorMessage(apiResponse.detail);
            }
        }
    }
    public IEnumerator LoginRequest(string endpoint, string username, string password)
    {
        string url = $"{baseURL}{endpoint}";

        string json = JsonUtility.ToJson(new UserRegister { username = username, password = password });

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.certificateHandler = new AcceptAllCertificatesSignedWithASelfSignedCertificate();

            yield return request.SendWebRequest();
            string responseText = request.downloadHandler.text;
            ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(responseText);

            if (request.result == UnityWebRequest.Result.Success)
            {
                menuManager.OnSuccessMessage(apiResponse.detail);
                menuManager.LoggedIn();
            }
            else
            {
                menuManager.OnErrorMessage(apiResponse.detail);
            }
        }
    }
    public void Register(string username, string password)
    {
        StartCoroutine(RegisterRequest("register/", username, password));
    }
    public void Login(string username, string password)
    {
        StartCoroutine(LoginRequest("login/", username, password));
    }

    [System.Serializable]
    public class UserRegister
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class ApiResponse
    {
        public string error;
        public string detail;
        public UserRegister userdata;
    }
}
