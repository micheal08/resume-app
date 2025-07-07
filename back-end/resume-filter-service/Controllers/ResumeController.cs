using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using resume_filter_service.Services.Interfaces;

namespace resume_filter_service.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeController : ControllerBase
    {
        private readonly ITextExtractionService _textExtractionService;
        private readonly IScoringService _scoringService;
        public ResumeController(ITextExtractionService textExtraction, IScoringService scoring)
        {
            _textExtractionService = textExtraction;
            _scoringService = scoring;
        }

        [HttpGet("secure")]
        public IActionResult GetSecureData()
        {
            return Ok("You are authenticated!");
        }

        [HttpPost("match-resumes")]
        public async Task<IActionResult> MatchResumes(IFormFile jdFile, List<IFormFile> resumes)
        {
            if (jdFile == null || resumes == null || resumes.Count == 0)
                return BadRequest("Please provide job description and resume files.");

            using var jdStream = jdFile.OpenReadStream();
            var jdText = await _textExtractionService.ExtractTextAsync(jdStream);

            var resumeTextMap = new Dictionary<string, string>();

            foreach (var resume in resumes)
            {
                using var resumeStream = resume.OpenReadStream();
                var text = await _textExtractionService.ExtractTextAsync(resumeStream);
                resumeTextMap.Add(resume.FileName, text);
            }

            var scores = await _scoringService.CalculateScoresAsync(jdText, resumeTextMap);

            return Ok(scores);

        }
    }
}
