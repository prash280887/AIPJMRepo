using ReportingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportingService.Services
{
    public interface IMailerService
    {
        public Task SendEmailWithAttachmentAsync(MailRequest mailRequest);
      //  public Task SendEmailWithAttachmentAsync(string content , AzurePromptRequest azurePromptRequest);
    }
}
