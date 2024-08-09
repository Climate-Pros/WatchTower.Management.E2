namespace WatchTower.Management.ServiceModel.Types;


/// <summary>
/// Configuration for sending emails using SMTP servers in EmailServices
/// E.g. for managed services like Amazon (SES): https://aws.amazon.com/ses/ or https://mailtrap.io
/// </summary>
public class SmtpConfig
{
    /// <summary>
    /// Username of the SMTP Server account
    /// </summary>
    public string Username { get; set; }
    /// <summary>
    /// Password of the SMTP Server account
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// Hostname of the SMTP Server
    /// </summary>
    public string Host { get; set; }
    /// <summary>
    /// Port of the SMTP Server
    /// </summary>
    public int Port { get; set; } = 587;
    /// <summary>
    /// Which email address to send emails from
    /// </summary>
    public string FromEmail { get; set; }
    /// <summary>
    /// The name of the Email Sender
    /// </summary>
    public string? FromName { get; set; }
    /// <summary>
    /// Prevent emails from being sent to real users during development by sending to this Dev email instead
    /// </summary>
    public string? DevToEmail { get; set; }
    /// <summary>
    /// Keep a copy of all emails sent by BCC'ing a copy to this email address
    /// </summary>
    public string? Bcc { get; set; }
}