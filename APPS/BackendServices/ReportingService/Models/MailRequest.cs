using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportingService.Models
{
    public class MailRequest
    {
        // Custome mAil request props - currennty meant for report send only
        public AzurePromptRequest? PromptRequest { get; set; }

        // Communication type (EMAIL, SMS, etc.)
        public string Type { get; set; } = "EMAIL";

        // emaail Subject
        public string? Subject { get; set; }


        // Email/SMS content
        public string? Content { get; set; }


        // Email/SMS html content
        public string? HtmlContent { get; set; }

        // Recipients
        public List<string>? To { get; set; }

        // Recipients
        public List<string>? Cc { get; set; }

        // Recipients
        public List<string>? Bcc { get; set; }
        // Email formatting
        public bool IsHtml { get; set; } = true;

        public MailAttachment[] Attachments { get; set; } = Array.Empty<MailAttachment>();
    }


}
