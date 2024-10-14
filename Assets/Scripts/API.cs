using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class API : MonoBehaviour
{
    private static string baseURL = "https://52.58.160.54/";

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

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log(responseText);
            }
            else
            {
                Debug.LogError($"Error: {request.error} | Response: {request.downloadHandler.text}");
            }
        }
    }

    public void Register(string username, string password)
    {
        StartCoroutine(RegisterRequest("register/", username, password));
    }

    [System.Serializable]
    public class UserRegister
    {
        public string username;
        public string password;
    }
}
