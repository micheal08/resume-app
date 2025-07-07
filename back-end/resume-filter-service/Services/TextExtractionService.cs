using Azure;
using Azure.AI.DocumentIntelligence;
using resume_filter_service.Services.Interfaces;
using System.Text;

namespace resume_filter_service.Services
{
    public class TextExtractionService : ITextExtractionService
    {
        private readonly DocumentIntelligenceClient _client;
        public TextExtractionService(IConfiguration config) 
        {
            var endpoint = config["FormRecognizer:Endpoint"];
            var key = config["FormRecognizer:Key"];
            var credential = new AzureKeyCredential(key);
            _client = new DocumentIntelligenceClient(new Uri(endpoint), credential);
        }
        public async Task<string> ExtractTextAsync(Stream fileStream)
        {
            BinaryData binaryData = await BinaryData.FromStreamAsync(fileStream);
            var options = new AnalyzeDocumentOptions("prebuilt-layout", binaryData);
            var operation = await _client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                options);
            var result = operation.Value;

            var resultText = new StringBuilder();
            foreach (var item in result.Pages)
            {
                foreach (var word in item.Words)
                {
                    resultText.AppendLine(word.Content);
                }
            }

            return resultText.ToString().Trim();
        }
    }
}
