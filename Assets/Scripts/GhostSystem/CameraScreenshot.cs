using System.Collections;
using UnityEngine;

public class CameraScreenshot : MonoBehaviour
{
    public Camera targetCamera;

    [SerializeField] private int targetWidth = 480;
    [SerializeField] private int targetHeight = 800;
    [SerializeField] private int frameRate = 20;
    [SerializeField] private int duration = 6;

    private void Start()
    {
        if (targetCamera == null)
        {
            Debug.LogError("Target Camera not assigned!");
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            StartCoroutine(CaptureScreenshotsForDuration());
        }
    }

    private IEnumerator CaptureScreenshotsForDuration()
    {
        var startTime = Time.time;
        var endTime = startTime + duration;

        while (Time.time < endTime)
        {
            CaptureScreenshotFromCamera();
            yield return new WaitForSeconds(1f / frameRate);
        }

        Debug.Log("Screenshot capture sequence complete!");
    }

    private void CaptureScreenshotFromCamera()
    {
        var renderTexture = new RenderTexture(targetWidth, targetHeight, 24);
        targetCamera.targetTexture = renderTexture;

        var screenshotTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);

        targetCamera.Render();
        RenderTexture.active = renderTexture;
        screenshotTexture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        var screenshotBytes = screenshotTexture.EncodeToJPG();

        var filename = "Assets/Screenshots/screenshot_" + Time.frameCount.ToString("0000") + ".jpg";
        System.IO.File.WriteAllBytes(filename, screenshotBytes);
    }
}