using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrintManager : SingletonDestroyable<PrintManager>
{
    private Animator m_Animator;
    private RawImage m_Background;
    private TMP_InputField m_InputField;
    private Button m_SendButton;
    private Button m_CloseButton;
    private GameObject m_WaterMarkOverlay;
    private Texture2D m_EmailPrint;

    [Header("Result Panels")]
    public GameObject sucessfullPanel;
    public GameObject failedPanel;
    public GameObject closeButton;

    // Email Data
    private const string FromEmail = "joao.santos@hivolve.com";
    private const string EmailPassword = "hijoaosantos";
    private string ToEmail;

    private bool ErrorSending;
    private bool EmailSent
    {
        set
        {
            if (value == true)
            {
                if (ErrorSending)
                {
                    failedPanel.SetActive(true);
                }
                else
                {
                    sucessfullPanel.SetActive(true);
                }
            }

            ClosePrintPanel();
            closeButton.SetActive(true);
        }
    }
    private MemoryStream m_MemoryStream;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Background = transform.GetChild(0).GetComponent<RawImage>();
        m_InputField = transform.GetChild(1).GetComponent<TMP_InputField>();
        m_SendButton = transform.GetChild(2).GetComponent<Button>();
        m_CloseButton = transform.GetChild(3).GetComponent<Button>();
        m_WaterMarkOverlay = transform.GetChild(4).gameObject;

        m_SendButton.onClick.AddListener(SendEmail);
    }
    private void Start()
    {
        CheckEmailInput();
    }

    public void SetPrintImage(Texture2D texture)
    {
        m_Background.texture = texture;
        m_EmailPrint = texture;
    }
    public void CheckEmailInput() => m_SendButton.interactable = IsValidEmail(m_InputField.text);
    public void OpenPrintPanel() => m_Animator.SetBool("Visible", true);
    public void ClosePrintPanel() => m_Animator.SetBool("Visible", false);
    public void SetOverlayText(string text) => m_WaterMarkOverlay.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
    public void SetOverlayVisibility(bool value) => m_WaterMarkOverlay.SetActive(value);
            

    private void SendEmail()
    {
        // Disable send button
        m_SendButton.interactable = false;
        m_CloseButton.interactable = false;

        // Get email to send to
        ToEmail = m_InputField.text;

        // Turn texture into file
        byte[] bytes = m_EmailPrint.EncodeToPNG();

        // Set email settings
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(FromEmail);
        mail.To.Add(ToEmail);
        mail.Subject = "Hivolve App Screenshot";
        mail.Body = "";
        string usertoken = "Confirmation Email";
        m_MemoryStream = new MemoryStream(bytes);
        try
        {
            mail.Attachments.Add(new Attachment(m_MemoryStream, "screenshot.png"));

            // you can use others too.
            SmtpClient smtpServer = new SmtpClient("smtp.outlook.com");
            smtpServer.Port = 587;
            smtpServer.EnableSsl = true;
            smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpServer.UseDefaultCredentials = false;
            smtpServer.Credentials = new NetworkCredential(FromEmail, EmailPassword);
            ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                Debug.Log("Returned");
                return true;
            };

            smtpServer.SendCompleted += new
            SendCompletedEventHandler(SendCompletedCallback);

            try
            {
                smtpServer.SendAsync(mail, usertoken);
            }
            catch (Exception e)
            {
                Debug.Log("Error");
                Debug.Log(e.GetBaseException());
            }
        }
        catch
        {
            Debug.Log("Failed Sending Email");
        }
    }
    private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        // Get the unique identifier for this asynchronous operation.
        String token = (string)e.UserState;

        if (e.Cancelled)
        {
            Debug.Log("[{0}] Send canceled." + token);
        }
        if (e.Error != null)
        {
            Debug.Log("[{0}] {1}" + token + e.Error.ToString());
            ErrorSending = true;
        }
        else
        {
            Debug.Log("Message sent.");
            ErrorSending = false;
        }
        EmailSent = true;
    }
    private static bool IsValidEmail(string email)
    {
        var rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
        return rx.IsMatch(email);
    }
}