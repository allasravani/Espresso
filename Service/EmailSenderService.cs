using ESPRESSO.Interfaces;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Logging;

public class EmailSenderService : IEmailSenderService
{
    private readonly AppContext _dbContext;
    private readonly IConfiguration _configuration;

    public EmailSenderService(AppContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }


    public void SendEmails()
    {
        var urlDatas = _dbContext.PageCounters.ToList();

        /* var urlDatas1 = _dbContext.PageCounters
              .Where(pc => pc.LAST_UPDATED_DATE_AND_TIME.Date == postgresTimestamp && pc.EMAIL != null)
              .ToList();*/

        foreach (var url in urlDatas)
        {
            if (url.LAST_UPDATED_DATE_AND_TIME == DateTime.UtcNow.Date)
            {
                string emailSubject = "Number of users accessed the stream " + url.STREAM_NAME;

                string body = $@"
        <html>
        <body>
            <p>Dear User,</p>
            <p>We wanted to inform you about the recent activity on the {url.STREAM_NAME} stream.</p>
            <table border='1' style='border-collapse: collapse; width: 100%;'>
                <tr>
                    <th style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>Stream Name</th>
                    <th style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>Stream Url</th>
                    <th style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>Count</th>
                </tr>
                <tr>
                    <td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>{url.STREAM_NAME}</td>
                    <td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>{url.Url}</td>
                    <td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>{url.COUNT}</td>
                </tr>
            </table>
            <p>Thank you for using our service!</p>
            <p>Regards,<br>WeTech Team</p>
        </body>
        </html>";
                if (!string.IsNullOrEmpty(url.EMAIL))
                    SendEmail(url.EMAIL, emailSubject, body);
            }
        }
    }
    public void SendEmail(string to, string subject, string body)
    {
        try
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            if (smtpSettings == null || !smtpSettings.GetChildren().Any())
            {
                  Console.WriteLine("SmtpSettings section is missing or empty.");
                return;
            }
            else
            {
                string? server = smtpSettings["Server"];
                string? portStr = smtpSettings["Port"];
                string? username = smtpSettings["Username"];
                string? password = smtpSettings["Password"];
                string? enableSslStr = smtpSettings["EnableSsl"];

                if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(portStr) ||
                    string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(enableSslStr))
                {
                    Console.WriteLine("One or more required configuration values are missing.");
                    return;
                }

                if (!int.TryParse(portStr, out int port))
                {
                    Console.WriteLine("Port is not a valid integer.");
                    return;
                }

                using (SmtpClient client = new SmtpClient())
                {
                    client.Host = server;
                    client.Port = port;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(username, password);

                    if (!bool.TryParse(enableSslStr, out bool enableSsl))
                    {
                        Console.WriteLine("EnableSsl is not a valid boolean.");
                        return;
                    }

                    client.EnableSsl = enableSsl;
                    client.Timeout = 10000;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(username);
                        mailMessage.To.Add(to);
                        mailMessage.Body = body;
                        mailMessage.Subject = subject;
                        mailMessage.IsBodyHtml = true;
                        client.Send(mailMessage);
                    }
                }
            }
        }
        catch(Exception)
        {
            throw;
        }
    }
}

