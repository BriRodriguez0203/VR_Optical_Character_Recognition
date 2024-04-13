using UnityEngine;
using System.Collections;

public class CameraCapture : MonoBehaviour
{
    private Camera _camera;

    private string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
    private string folderName = "UnityCaptures";
    private string savePath;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        savePath = System.IO.Path.Combine(desktopPath, folderName);

        if (!System.IO.Directory.Exists(savePath))
        {
            System.IO.Directory.CreateDirectory(savePath);
        }

        if (_camera.gameObject.layer != LayerMask.NameToLayer("Board"))
        {
            this.enabled = false;
        }
    }

    public IEnumerator CaptureAndSavePNG()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture renderTexture = _camera.targetTexture;

        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            _camera.targetTexture = renderTexture;
        }

        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
        _camera.Render();
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        byte[] imageBytes = texture.EncodeToPNG();
        string fileName = System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        string fullPath = System.IO.Path.Combine(savePath, fileName);
        System.IO.File.WriteAllBytes(fullPath, imageBytes);

        Debug.Log("Saved Image to " + fullPath);

        RenderTexture.active = null;
        _camera.targetTexture = null;
        Destroy(texture);
    }
}
