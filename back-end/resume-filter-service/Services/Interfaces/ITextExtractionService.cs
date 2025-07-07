namespace resume_filter_service.Services.Interfaces
{
    public interface ITextExtractionService
    {
        Task<string> ExtractTextAsync(Stream fileStream);
    }
}
