using Markdig.Parsers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReportingService.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;

namespace ReportingService.Services
{
    public class MailerService : IMailerService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public MailerService(IHttpClientFactory httpFactory, ILogger<ReportingFunctions> logger, IConfiguration config)
        {
            _config = config;
            _httpFactory = httpFactory;
             _logger = logger;
        }

        //public async Task SendEmailWithAttachmentAsync(string contentAsAttachment, AzurePromptRequest azurePromptRequest)
        //{
        //    var sendGridKey = _config["SendGridApiKey"]
        //        ?? throw new InvalidOperationException("SendGridApiKey not configured");

        //    var client = new SendGridClient(sendGridKey);

        //    string sender = _config["SmtpFromEmail"]
        //        ?? throw new InvalidOperationException("SmtpFromEmail not configured");

        //    var from = new EmailAddress(sender, "aipjm Test Reports");

        //    // TO
        //    var toEmails = (_config["SmtpToEmail"] ?? "")
        //        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        //        .Select(e => new EmailAddress(e.Trim()))
        //        .ToList();

        //    // CC
        //    var ccEmails = (_config["SmtpCcEmail"] ?? "")
        //        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        //        .Select(e => new EmailAddress(e.Trim()))
        //        .ToList();

        //    // BCC
        //    var bccEmails = (_config["SmtpBccEmail"] ?? "")
        //        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        //        .Select(e => new EmailAddress(e.Trim()))
        //        .ToList();

        //    var subject = $"aipjm - Automated Project Report ({DateTime.UtcNow:yyyy-MM-dd}) - " + azurePromptRequest.azureBoardsConfig?.ProjectDisplayName;
        //    var plainTextContent = "Please find attached the automated report for project - " + azurePromptRequest.azureBoardsConfig?.ProjectDisplayName;
        //    var htmlContent = "<p>Please find attached automated report for project - "+ azurePromptRequest.azureBoardsConfig?.ProjectDisplayName + "<br/><br/>Regards<br />WinBuild1 IntelliTrace Team</p>";

        //    // Use CreateSingleEmailToMultipleRecipients
        //    var msg = MailHelper.CreateSingleEmailToMultipleRecipients(
        //        from,
        //        toEmails,
        //        subject,
        //        plainTextContent,
        //        htmlContent
        //    );

        //    // Add CC & BCC via Personalizations
        //    var personalization = msg.Personalizations.First();

        //    if (ccEmails.Any())
        //        personalization.Ccs = ccEmails;

        //    if (bccEmails.Any())
        //        personalization.Bccs = bccEmails;

        //    // Attachment
        //    var bytes = Encoding.UTF8.GetBytes(content);
        //    var attachment = Convert.ToBase64String(bytes);

        //    msg.AddAttachment(
        //        $"aipjm-{DateTime.UtcNow:yyyyMMdd}.html",
        //        attachment,
        //        "text/html"
        //    );

        //    var response = await client.SendEmailAsync(msg);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var body = await response.Body.ReadAsStringAsync();
        //        throw new InvalidOperationException(
        //            $"SendGrid failed: {response.StatusCode} - {body}"
        //        );
        //    }
        //}

        public async Task SendEmailWithAttachmentAsync(MailRequest mailRequest)
        {
            var sendGridKey = _config["SendGridApiKey"]
                ?? throw new InvalidOperationException("SendGridApiKey not configured");

            var client = new SendGridClient(sendGridKey);

            string sender = _config["SmtpFromEmail"]
                ?? throw new InvalidOperationException("SmtpFromEmail not configured");

            var from = new EmailAddress(sender, "aipjm Test Reports");

            // TO
            var toEmails = (_config["SmtpToEmail"] ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => new EmailAddress(e.Trim()))
                .ToList();

            //include recepients from ui (default login user) - not tested , clean empty or duplicates          
            toEmails.AddRange(mailRequest.To?.Select(e => new EmailAddress(e.Trim())) ?? Enumerable.Empty<EmailAddress>());
            toEmails = toEmails.DistinctBy(e => e.Email).ToList();

            // CC
            var ccEmails = (_config["SmtpCcEmail"] ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => new EmailAddress(e.Trim()))
                .ToList();

            //include recepients from ui   
            // ccEmails.AddRange(mailRequest.Cc?.Select(e => new EmailAddress(e.Trim())) ?? Enumerable.Empty<EmailAddress>());
           // ccEmails = ccEmails.DistinctBy(e => e.Email).ToList();

            // BCC
            var bccEmails = (_config["SmtpBccEmail"] ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => new EmailAddress(e.Trim()))
                .ToList();

            //  include recepients from ui        
            // bccEmails.AddRange(mailRequest.Bcc?.Select(e => new EmailAddress(e.Trim())) ?? Enumerable.Empty<EmailAddress>());
           // bccEmails = bccEmails.DistinctBy(e => e.Email).ToList();


            var subject = string.IsNullOrEmpty(mailRequest.Subject) ?  $"aipjm - Automated Project Report ({DateTime.UtcNow:yyyy-MM-dd}) - " + mailRequest?.PromptRequest?.azureBoardsConfig?.ProjectDisplayName : mailRequest.Subject;
            var plainTextContent = string.IsNullOrEmpty(mailRequest.Content) ?  "Please find attached the AI generated automated report for project - " + mailRequest?.PromptRequest?.azureBoardsConfig?.ProjectDisplayName : mailRequest.Content;
            var htmlContent = string.IsNullOrEmpty(mailRequest.HtmlContent) ? "<p>Please find attached AI generated automated report for project - " + mailRequest?.PromptRequest?.azureBoardsConfig?.ProjectDisplayName + "<br/><br/>Regards<br />WinBuild1 IntelliTrace Team</p>" : mailRequest.HtmlContent;

            // Use CreateSingleEmailToMultipleRecipients
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(
                from,
                toEmails,
                subject,
                plainTextContent,
                htmlContent
            );

            // Add CC & BCC via Personalizations
            var personalization = msg.Personalizations.First();

            if (ccEmails.Any())
                personalization.Ccs = ccEmails;

            if (bccEmails.Any())
                personalization.Bccs = bccEmails;

            // Attachment
           foreach(var attachmentItem in mailRequest.Attachments)
            {
                // Attachment
                var bytes = Encoding.UTF8.GetBytes(attachmentItem.ContentsAsAttachment);
                var attachment = Convert.ToBase64String(bytes);
                var fileName = string.IsNullOrEmpty(attachmentItem.FileName) ? $"aipjm-{DateTime.UtcNow:yyyyMMdd}.{attachmentItem.FileExtension}" : $"aipjm-{DateTime.UtcNow:yyyyMMdd}.html";
                var mimeType = string.IsNullOrEmpty(attachmentItem.MimeType) ? "text/html" : attachmentItem.MimeType;
                msg.AddAttachment(fileName, attachment, attachmentItem.MimeType);
            }

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new InvalidOperationException(
                    $"SendGrid failed: {response.StatusCode} - {body}"
                );
            }
        }

        private List<EmailAddress> ParseEmailAddresses(string emailString)
        {
            return (emailString ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct()
                .Select(e => new EmailAddress(e))
                .ToList();
        }
    }
}
