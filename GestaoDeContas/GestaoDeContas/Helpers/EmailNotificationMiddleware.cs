using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using GestaoDeContas.Data;

namespace GestaoDeContas.Helpers;

public class EmailNotificationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public EmailNotificationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/")
        {
            var currentDate = DateTime.UtcNow;
            if (currentDate.Day <= 10)
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var contasVencendo = await dbContext.ContasPagar
                    .Where(c => c.DataVencimento.Year == currentDate.Year && c.DataVencimento.Month == currentDate.Month)
                    .ToListAsync();

                if (contasVencendo.Count != 0)
                {
                    var emailBody = contasVencendo
                        .Aggregate("Contas a vencer esse mês:\n\n", (current, conta) => current + 
                            $"Descrição: {conta.Descricao}, Valor: {conta.Valor}, Data de Vencimento: {conta.DataVencimento.ToShortDateString()}\n");
                    SendEmail(emailBody);
                }
            }
        }

        await _next(context);
    }

    private static void SendEmail(string body)
    {
        const string subject = "Contas a Vencer - Mês";
        
        var smtpClient = new SmtpClient("smtp.gmail.com", 587);
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential($"{Environment.GetEnvironmentVariable("USERNAME")}", 
            $"{Environment.GetEnvironmentVariable("PASSWORD")}");
        
        var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress($"{Environment.GetEnvironmentVariable("EMAIL_FROM")}");
        mailMessage.To.Add($"{Environment.GetEnvironmentVariable("EMAIL_TO")}");
        mailMessage.Subject = subject;
        mailMessage.Body = body;
        smtpClient.Send(mailMessage);
    }
}