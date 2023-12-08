using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using ESPRESSO.Models;
using ESPRESSO.Interfaces;

[Route("api/Email")]
[ApiController]
public class EmailController : ControllerBase //,IEmailSenderService
{
    private readonly AppContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly EmailSenderService _emailSenderService;

    public EmailController(AppContext dbContext, IConfiguration configuration, EmailSenderService emailSenderService)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _emailSenderService = emailSenderService;
    }

    [HttpPost("emailSend")]
    public IActionResult EmailSend([FromBody] EmailModel model)
    {
        try
        {
            emailTrigger(model.To, model.Subject, model.Body);
            return Ok("Email sent successfully.");
        }
        catch (Exception ex)
        {
            return Ok(ex.Message);
        }
    }


    [NonAction]
    private void emailTrigger(string to, string subject, string body)
    {
        _emailSenderService.SendEmail( to,  subject,  body);
    }
    
    
}
