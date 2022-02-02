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
        yield return new WaitForEndOfFrame();

        // Set file name and location
        m_FileName = "Screenshot" + System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Second + ".png";
        m_DefaultPath = Application.persistentDataPath + "/" + m_FileName;

        // Hide hud buttons if option is selected
        if (AppManager.Instance.hideHudWhenTakingPrintscreen)
            HudManager.Instance.SetButtonsVisibility(false);
        if (AppManager.Instance.applyOverlay)
            PrintManager.Instance.SetOverlayVisibility(true);

        // Wait to end of frame so the texture is applied
        //yield return null;
        //yield return new WaitForEndOfFrame();

        yield return null;
        yield return new WaitForEndOfFrame();

        var screenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();
        //Texture2D image = new Texture2D(Camera.main.targetTexture.width, Camera.main.targetTexture.height, TextureFormat.RGB565, false);
        //var image = ScreenCapture.CaptureScreenshotAsTexture();
        //image.ReadPixels(new Rect(0, 0, m_CameraRenderTexture.width, m_CameraRenderTexture.height), 0, 0);
        //image.Apply();
        //RenderTexture.active = null;

        //Camera.main.targetTexture = null;
        // Turn hud buttons back on
        if (AppManager.Instance.hideHudWhenTakingPrintscreen)
            HudManager.Instance.SetButtonsVisibility(true);
        if (AppManager.Instance.applyOverlay)
            PrintManager.Instance.SetOverlayVisibility(false);

        // Set image on UI
        PrintManager.Instance.SetPrintImage(screenshotTexture);

        // Open UI
        PrintManager.Instance.OpenPrintPanel();
    }
}