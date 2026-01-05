namespace Marketplace_LabWebBD.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
        Task SendEmailConfirmationAsync(string toEmail, string nome, string confirmationLink);
        Task SendVendorApprovalNotificationAsync(string toEmail, string nome, bool approved, string? rejectReason = null);
        Task SendAccountBlockedNotificationAsync(string toEmail, string nome, string motivo);
        Task SendWelcomeEmailAsync(string toEmail, string nome, string role);
    }
}
