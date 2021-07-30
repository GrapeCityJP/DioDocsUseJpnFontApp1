using GrapeCity.Documents.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace DioDocsUseJpnFontApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string Message = string.IsNullOrEmpty(name)
                ? "Ç±ÇÒÇ…ÇøÇÕÅAê¢äEÅI"
                : $"Ç±ÇÒÇ…ÇøÇÕÅA{name}ÅI";

            //Workbook.SetLicenseKey("");

            Workbook workbook = new Workbook();

            workbook.Worksheets[0].Range["A1"].Font.Name = "ÇlÇr Çoñæí©";

            workbook.Worksheets[0].Range["A1"].Value = Message;

            byte[] output;

            using (var ms = new MemoryStream())
            {
                workbook.Save(ms, SaveFileFormat.Xlsx);
                output = ms.ToArray();
            }

            return new FileContentResult(output, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "Result.xlsx"
            };
        }
    }
}
