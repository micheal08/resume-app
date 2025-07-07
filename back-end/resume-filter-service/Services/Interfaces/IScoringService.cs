using resume_filter_service.Models;

namespace resume_filter_service.Services.Interfaces
{
    public interface IScoringService
    {
        Task<List<ResumeScore>> CalculateScoresAsync(string jdText, Dictionary<string, string> resumes);
    }
}
