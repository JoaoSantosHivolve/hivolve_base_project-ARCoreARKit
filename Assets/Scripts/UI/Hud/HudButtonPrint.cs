using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudButtonPrint : HudButton
{
    private string m_FileName;
    private string m_DefaultPath;
    //private Texture2D m_ScreenshotTexture;
    private RenderTexture m_CameraRenderTexture;

    private void Start()
    {
        m_CameraRenderTexture = Resources.Load<RenderTexture>("AR/Images/CameraRenderTexture/CameraRenderTexture");
    }

    protected override void CheckIfActivated()
    {
    }

    protected override void OnClick()
    {
        StartCoroutine(TakePrintScreen());
    }

    private IEnumerator TakePrintScreen()
    {
        // Set file name and location
        m_FileName = "Screenshot" + System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Second + ".png";
        m_DefaultPath = Application.persistentDataPath + "/" + m_FileName;

        ////// Take screenshot
        ////m_ScreenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();
        ////var nTex = new Texture2D(m_ScreenshotTexture.width, m_ScreenshotTexture.height,TextureFormat.RGB565, false);
        ////Graphics.CopyTexture(m_ScreenshotTexture, nTex);

        // Add render texture to camera
        Camera.main.targetTexture = m_CameraRenderTexture;

        // Hide hud buttons if option is selected
        //if (AppManager.Instance.hideHudWhenTakingPrintscreen)
        //    HudManager.Instance.SetButtonsVisibility(false);
        //if (AppManager.Instance.applyOverlay)
        //    HudManager.Instance.SetButtonsVisibility(true);

        // Wait to end of frame so the texture is applied
        yield return new WaitForEndOfFrame();

        // Copy camera's render texture into a Texture2D
        RenderTexture.active = Camera.main.targetTexture;
        Texture2D image = new Texture2D(Camera.main.targetTexture.width, Camera.main.targetTexture.height, TextureFormat.RGB565, false);
        image.ReadPixels(new Rect(0, 0, Camera.main.targetTexture.width, Camera.main.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = null;

        Camera.main.targetTexture = null;
        // Turn hud buttons back on
        //if (AppManager.Instance.hideHudWhenTakingPrintscreen)
        //    HudManager.Instance.SetButtonsVisibility(true);
        //if (AppManager.Instance.applyOverlay)
        //    HudManager.Instance.SetButtonsVisibility(false);

        // Set image on UI
        PrintManager.Instance.SetPrintImage(image);

        // Open UI
        PrintManager.Instance.OpenPrintPanel();
    }
}