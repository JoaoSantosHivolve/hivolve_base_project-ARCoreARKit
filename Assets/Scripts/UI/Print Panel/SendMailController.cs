using System;
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
public class SendMailController : MonoBehaviour
{
    public Texture2D texture;
    public TMP_InputField inputField;
    public Button sendEmailButton;
    public Button closePanelButton;
    public Animator printPanelAnimator;

    [Header("Result Panels")]
    public GameObject sucessfullPanel;
    public GameObject failedPanel;
    public GameObject closeButton;

    // Email Data
    private const string FromEmail = "ooty@ootyme.com";
    private const string EmailPassword = "qbghpajutnncukmq";
    //private const string EmailPassword = "Teste159*";
    private string ToEmail;

    private bool ErrorSending;
    private bool EmailSent
    {
        set
        {
            if(value  == true)
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
        sendEmailButton.onClick.AddListener(SendEmail);
    }

    private void Start()
    {
        CheckEmailInput();
    }

    public void CheckEmailInput()
    {
        sendEmailButton.interactable = IsValidEmail(inputField.text);
    }

    private void SendEmail()
    {
        // Disable send button
        sendEmailButton.interactable = false;
        closePanelButton.interactable = false;

        // Get email to send to
        ToEmail = inputField.text;

        // Turn texture into file
        byte[] bytes = texture.EncodeToPNG();

        // Set email settings
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(FromEmail);
        mail.To.Add(ToEmail);
        mail.Subject = "Ooty Screenshot";
        mail.CC.Add("ooty@ootyme.com");
        mail.Body = "";
        string usertoken = "Confirmation Email";
        m_MemoryStream = new MemoryStream(bytes);
        try
        {
            mail.Attachments.Add(new Attachment(m_MemoryStream, "OotyScreenshot.png"));

            // you can use others too.
            SmtpClient smtpServer = new SmtpClient("smtp.yandex.com");
            smtpServer.Port = 25;
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
            Debug.Log("[{0}] Send canceled." +  token);
        }
        if (e.Error != null)
        {
            Debug.Log("[{0}] {1}" +  token + e.Error.ToString());
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
    public void ClosePrintPanel()
    {
        printPanelAnimator.SetBool("Visible", false);
    }
}