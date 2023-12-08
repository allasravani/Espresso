using ESPRESSO.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;
using Npgsql;

public class EmailService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IServiceProvider serviceProvider, ILogger<EmailService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
           var now = DateTime.Now;
             var nextRun = now.Date.AddHours(11).AddMinutes(30);

            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            var delay = nextRun - now;

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var emailSenderService = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();
                    emailSenderService.SendEmails();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing emails.");
            }

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}

