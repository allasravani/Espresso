using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Add this namespace
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System;
using NETCore.MailKit.Core;
using ESPRESSO;
using ESPRESSO.Interfaces;
using Npgsql;
//using ESPRESSO.Service;
//using EmailService = ESPRESSO.EmailService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
});
builder.Configuration.AddJsonFile("appsettings.json");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
//Scheduler
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddHostedService<EmailService>();
//builder.Services.AddSingleton<IEmailSenderService, EmailSenderService>();
//builder.Services.AddHostedService<EmailService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddSingleton<EmailController>();
//builder.Services.AddScoped<EmailController>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseRouting(); // Add this for routing

app.MapControllers();
app.UseStaticFiles();
app.Run();


