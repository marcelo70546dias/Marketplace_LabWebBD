using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Marketplace_LabWebBD.Models;

namespace Marketplace_LabWebBD.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            try
            {
                using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = _emailSettings.EnableSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email enviado com sucesso para {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar email para {toEmail}");
                throw;
            }
        }

        public async Task SendEmailConfirmationAsync(string toEmail, string nome, string confirmationLink)
        {
            var subject = "Confirme o seu email - MyOnlineStand";
            var htmlMessage = GetEmailConfirmationTemplate(nome, confirmationLink);
            await SendEmailAsync(toEmail, subject, htmlMessage);
        }

        public async Task SendVendorApprovalNotificationAsync(string toEmail, string nome, bool approved, string? rejectReason = null)
        {
            var subject = approved
                ? "Pedido de Vendedor Aprovado - MyOnlineStand"
                : "Pedido de Vendedor Rejeitado - MyOnlineStand";

            var htmlMessage = GetVendorApprovalTemplate(nome, approved, rejectReason);
            await SendEmailAsync(toEmail, subject, htmlMessage);
        }

        public async Task SendAccountBlockedNotificationAsync(string toEmail, string nome, string motivo)
        {
            var subject = "Conta Bloqueada - MyOnlineStand";
            var htmlMessage = GetAccountBlockedTemplate(nome, motivo);
            await SendEmailAsync(toEmail, subject, htmlMessage);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string nome, string role)
        {
            var subject = "Bem-vindo ao MyOnlineStand";
            var htmlMessage = GetWelcomeTemplate(nome, role);
            await SendEmailAsync(toEmail, subject, htmlMessage);
        }

        // Templates HTML
        private string GetEmailConfirmationTemplate(string nome, string confirmationLink)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .button {{ display: inline-block; padding: 12px 30px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🚗 MyOnlineStand</h1>
        </div>
        <div class='content'>
            <h2>Olá, {nome}!</h2>
            <p>Obrigado por se registar no MyOnlineStand, o seu marketplace de confiança para compra e venda de automóveis.</p>
            <p>Para concluir o seu registo e começar a utilizar a plataforma, por favor confirme o seu endereço de email clicando no botão abaixo:</p>
            <div style='text-align: center;'>
                <a href='{confirmationLink}' class='button'>Confirmar Email</a>
            </div>
            <p style='font-size: 12px; color: #666; margin-top: 20px;'>Este link é válido por 24 horas. Se não solicitou este registo, pode ignorar este email.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 MyOnlineStand. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetVendorApprovalTemplate(string nome, bool approved, string? rejectReason)
        {
            if (approved)
            {
                return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .success-box {{ background: #d4edda; border: 1px solid #c3e6cb; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>✅ Pedido Aprovado</h1>
        </div>
        <div class='content'>
            <h2>Parabéns, {nome}!</h2>
            <div class='success-box'>
                <strong>O seu pedido para se tornar vendedor foi aprovado!</strong>
            </div>
            <p>Pode agora aceder à sua conta e completar o seu perfil de vendedor com os seguintes passos:</p>
            <ol>
                <li>Faça login na plataforma</li>
                <li>Complete o seu perfil de vendedor (Tipo, NIF, Dados de Faturação)</li>
                <li>Comece a criar os seus anúncios de automóveis</li>
            </ol>
            <p>Estamos entusiasmados por tê-lo como vendedor na nossa plataforma!</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 MyOnlineStand. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
            }
            else
            {
                return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #dc3545 0%, #c82333 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .warning-box {{ background: #f8d7da; border: 1px solid #f5c6cb; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>❌ Pedido Rejeitado</h1>
        </div>
        <div class='content'>
            <h2>Olá, {nome}</h2>
            <div class='warning-box'>
                <p>Lamentamos informar que o seu pedido para se tornar vendedor não foi aprovado neste momento.</p>
                {(string.IsNullOrEmpty(rejectReason) ? "" : $"<p><strong>Motivo:</strong> {rejectReason}</p>")}
            </div>
            <p>No entanto, a sua conta foi convertida para Comprador, o que lhe permite continuar a navegar e comprar automóveis na plataforma.</p>
            <p>Se tiver questões sobre esta decisão, por favor contacte a nossa equipa de suporte.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 MyOnlineStand. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
            }
        }

        private string GetAccountBlockedTemplate(string nome, string motivo)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #dc3545 0%, #c82333 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .alert-box {{ background: #f8d7da; border: 1px solid #f5c6cb; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>⚠️ Conta Bloqueada</h1>
        </div>
        <div class='content'>
            <h2>Olá, {nome}</h2>
            <div class='alert-box'>
                <p><strong>A sua conta foi bloqueada por um administrador.</strong></p>
                <p><strong>Motivo:</strong> {motivo}</p>
            </div>
            <p>Não poderá aceder à plataforma até que esta situação seja resolvida.</p>
            <p>Se acredita que isto é um erro ou deseja discutir esta decisão, por favor contacte a nossa equipa de suporte.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 MyOnlineStand. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetWelcomeTemplate(string nome, string role)
        {
            var roleDisplay = role switch
            {
                "Comprador" => "Comprador",
                "Vendedor" => "Vendedor",
                "Administrador" => "Administrador",
                _ => "Utilizador"
            };

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Bem-vindo!</h1>
        </div>
        <div class='content'>
            <h2>Olá, {nome}!</h2>
            <p>Bem-vindo ao MyOnlineStand! O seu email foi confirmado com sucesso.</p>
            <p>Registou-se como: <strong>{roleDisplay}</strong></p>
            <p>Agora pode aproveitar todas as funcionalidades da plataforma:</p>
            <ul>
                <li>Pesquisar automóveis de qualidade</li>
                <li>Criar e gerir os seus anúncios</li>
                <li>Interagir com vendedores e compradores</li>
                <li>Guardar os seus favoritos</li>
            </ul>
            <p>Obrigado por escolher o MyOnlineStand!</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 MyOnlineStand. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
