using Azure;
using Azure.AI.OpenAI;
using resume_filter_service.Models;
using resume_filter_service.Services.Interfaces;

namespace resume_filter_service.Services
{
    public class ScoringService : IScoringService
    {
        private readonly AzureOpenAIClient _client;
        private readonly string _embeddingDeployment;
        public ScoringService(IConfiguration config)
        {
            var endpoint = config["OpenAI:Endpoint"];
            var key = config["OpenAI:Key"];
            _embeddingDeployment = config["OpenAI:EmbeddingDeployment"];
            _client = new(new Uri(endpoint), new AzureKeyCredential(key));
        }
        public async Task<List<Models.ResumeScore>> CalculateScoresAsync(string jdText, Dictionary<string, string> resumes)
        {
            var jdEmbedding = await GetEmbeddingAsync(jdText);
            var results = new List<ResumeScore>();

            foreach (var resume in resumes)
            {
                var resumeEmbedding = await GetEmbeddingAsync(resume.Value);
                double similarity = CosineSimilarity(jdEmbedding, resumeEmbedding);
                results.Add(new ResumeScore
                {
                    Name = resume.Key,
                    Score = Math.Round(similarity * 100, 1)
                });
            }

            return results.OrderByDescending(r => r.Score).ToList();
        }

        private async Task<IReadOnlyList<float>> GetEmbeddingAsync(string input)
        {
            var embeddingClient = _client.GetEmbeddingClient(_embeddingDeployment);
            var response = await embeddingClient.GenerateEmbeddingAsync(input);
            return response.Value.ToFloats().ToArray();
        }

        private double CosineSimilarity(IReadOnlyList<float> a, IReadOnlyList<float> b)
        {
            double dot = 0, normA = 0, normB = 0;
            for (int i = 0; i < a.Count; i++)
            {
                dot += a[i] * b[i];
                normA += Math.Pow(a[i], 2);
                normB += Math.Pow(b[i], 2);
            }
            return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }
    }
}
