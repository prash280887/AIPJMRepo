using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportingService.Models
{
    internal class ReportData
    {
        public string projectTitle { get; set; }

        public string projectsMessage { get; set; }

        public string projectsMessageHtml{get; set; } 

        public string projectsMessageHtmlTasks{get; set; } 

        public string projectsMessageHtmlRisks{get; set; } 

        public string projectsMessageHtmlRecommendations{get; set; } 
        
    }
}
