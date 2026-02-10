using AgenticAIService.Models.Azure;
using Newtonsoft.Json;
using QuestPDF.Fluent;
namespace AgenticAIService
{

    public class ReportExportService
    {
        public string ExportHtml(string content) => content;

        //public byte[] ExportPdf(string html)
        //{
        //    return Document.Create(document =>
        //    {
        //        document.Page(page =>
        //        {
        //            page.Content().Html(html);
        //        });
        //    }).GeneratePdf();
        //}

        public byte[] ExportWord(string html)
        {
            // Use OpenXML SDK (placeholder)
            return System.Text.Encoding.UTF8.GetBytes(html);
        }

        //public string ExportJson(UnifiedReportModel model)
        //{
        //    return JsonConvert.SerializeObject(model, Formatting.Indented);
        //}
    }
}
    

