using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace DioDocsUseJpnFontApp1
{
    public static class Function2
    {
        [FunctionName("Function2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, Microsoft.Azure.WebJobs.ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string Message = string.IsNullOrEmpty(name)
                ? "こんにちは、世界！"
                : $"こんにちは、{name}！";

            //GcPdfDocument.SetLicenseKey("");

            GcPdfDocument doc = new GcPdfDocument();
            GcPdfGraphics g = doc.NewPage().Graphics;

            Font font = Font.FromFile(Path.Combine(context.FunctionAppDirectory, "fonts", "ipaexg.ttf"));

            g.DrawString(Message,
                new TextFormat() { Font = font, FontSize = 12 },
                new PointF(72, 72));

            byte[] output;

            using (var ms = new MemoryStream())
            {
                doc.Save(ms, false);
                output = ms.ToArray();
            }

            return new FileContentResult(output, "application/pdf")
            {
                FileDownloadName = "Result.pdf"
            };
        }
    }
}
