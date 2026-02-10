using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportingService.Models
{
    public class MailAttachment
    {
        public string ContentsAsAttachment { get; set; }

        public string FileName { get; set; }

        public string MimeType { get; set; }

        public string FileExtension { get; set; }

    }
}
