using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenshotHandler : MonoBehaviour
{
    private static ScreenshotHandler instance;
    private Camera screenshotCamera;
    private string path;
    private bool takeScreenshotOnNextFrame;
    private void Awake()
    {
        screenshotCamera = GetComponent<Camera>();
        instance = this;
    }

    private void OnPostRender()
    {
        if (takeScreenshotOnNextFrame)
        {
            takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = screenshotCamera.targetTexture;
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            File.WriteAllBytes(path + ".png", byteArray);

            RenderTexture.ReleaseTemporary(renderTexture);
            screenshotCamera.targetTexture = null;
        }
    }

    private void TakeScreenshot(int width, int height, string path)
    {
        screenshotCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        instance.path = path;
        takeScreenshotOnNextFrame = true;
    }

    public static void TakeScreenshot_Static(int width, int height, string path)
    {
        instance.TakeScreenshot(width, height, path);
    }
}

