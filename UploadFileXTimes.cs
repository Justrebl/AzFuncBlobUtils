using System.Net;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azFuncFileFlooder
{
    public class UploadFileXTimes
    {
        private readonly ILogger _logger;

        public UploadFileXTimes(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<UploadFileXTimes>();
        }

        [Function("UploadFileXTimes")]
        // [BlobOutput("zips/{rand-guid}", Connection = "AzureWebJobsStorage")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            String storageCntStr = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            String containerName = Environment.GetEnvironmentVariable("ContainerName");

            BlobContainerClient containerClient = new BlobContainerClient(storageCntStr, containerName);
            containerClient.CreateIfNotExists();

            Random random = new Random();

            await UploadFromByteArrayAsync(containerClient, GetByteContentFromRequest(), $"{random}test.zip");

            //Create an http response
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString("Welcome to Azure Functions!");

            return response;
        }

        public static async Task UploadFromByteArrayAsync(BlobContainerClient containerClient, byte[] blobContent, string fileName)
        {
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            Stream stream = new MemoryStream(blobContent);
            await blobClient.UploadAsync(stream, true);
        }

        public static byte[] GetByteContentFromRequest()
        {
            byte[] byteContent = new byte[0];
            return byteContent;
        }
    }
}
