using System.Net.Mail;
using Microsoft.Extensions.Logging;
using ServiceStack;
using WatchTower.Management.ServiceModel;
using WatchTower.Management.ServiceModel.Types;

namespace WatchTower.Management.ServiceInterface;


/// <summary>
/// Uses a configured SMTP client to send emails
/// </summary>
public class EmailServices(SmtpConfig config, ILogger<EmailServices> log) : Service
{
    public object Any(SendEmail request)
    {
        log.LogInformation("Sending email to {Email} with subject {Subject}", request.To, request.Subject);

        using var client = new SmtpClient(config.Host, config.Port);
        client.Credentials = new System.Net.NetworkCredential(config.Username, config.Password);
        client.EnableSsl = true;

        // If DevToEmail is set, send all emails to that address instead
        var emailTo = config.DevToEmail != null
            ? new MailAddress(config.DevToEmail)
            : new MailAddress(request.To, request.ToName);

        var emailFrom = new MailAddress(config.FromEmail, config.FromName);

        var msg = new MailMessage(emailFrom, emailTo)
        {
            Subject = request.Subject,
            Body = request.BodyHtml ?? request.BodyText,
            IsBodyHtml = request.BodyHtml != null,
        };

        if (config.Bcc != null)
        {
            msg.Bcc.Add(new MailAddress(config.Bcc));
        }

        client.Send(msg);

        return new EmptyResponse();
    }
}