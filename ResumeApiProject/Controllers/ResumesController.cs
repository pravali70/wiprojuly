using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeApi.DTOs;
using ResumeApi.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using System.IO;
using System.Text.Json;

namespace ResumeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "RegisteredUser")] 
    public class ResumesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ResumesController(AppDbContext db)
        {
            _db = db;
        }

        // ✅ Helper to extract UserId from JWT
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? throw new Exception("User ID not found in token");
        }

        // GET: api/resumes (returns only logged-in user's resumes)
        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            var uid = GetUserId();
            var list = await _db.Resumes
                .Where(r => r.UserId == uid)
                .ToListAsync();

            return Ok(list);
        }

        // GET: api/resumes/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var uid = GetUserId();
            var r = await _db.Resumes.FirstOrDefaultAsync(x => x.ResumeId == id && x.UserId == uid);

            if (r == null) return NotFound();
            return Ok(r);
        }

        // Download Resume as PDF
        [HttpGet("download/{id:int}")]
        public async Task<IActionResult> DownloadResume(int id)
        {
            var uid = GetUserId();
            var resume = await _db.Resumes.FirstOrDefaultAsync(r => r.ResumeId == id && r.UserId == uid);
            if (resume == null) return NotFound("Resume not found");

            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);

            doc.Add(new iText.Layout.Element.Paragraph($"Resume Title: {resume.Title}"));
            doc.Add(new iText.Layout.Element.Paragraph($"Personal Info: {resume.PersonalInfo}"));
            doc.Add(new iText.Layout.Element.Paragraph($"Education: {resume.Education}"));
            doc.Add(new iText.Layout.Element.Paragraph($"Experience: {resume.Experience}"));
            doc.Add(new iText.Layout.Element.Paragraph($"Skills: {resume.Skills}"));
            if (!string.IsNullOrEmpty(resume.AiSuggestions))
                doc.Add(new iText.Layout.Element.Paragraph($"AI Suggestions: {resume.AiSuggestions}"));

            doc.Close();
            return File(ms.ToArray(), "application/pdf", $"{resume.Title}_Resume.pdf");
        }

        // POST: api/resumes
        [HttpPost]
        public async Task<IActionResult> Create(ResumeDto dto)
        {
            var uid = GetUserId(); // take from token

            var r = new Resume
            {
                UserId = uid,
                Title = dto.Title,
                PersonalInfo = dto.PersonalInfo,
                Education = dto.Education,
                Experience = dto.Experience,
                Skills = dto.Skills,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _db.Resumes.Add(r);
            await _db.SaveChangesAsync();
            return Ok(r.ResumeId);
        }

        // PUT: api/resumes/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ResumeDto dto)
        {
            var uid = GetUserId();
            var r = await _db.Resumes.FirstOrDefaultAsync(x => x.ResumeId == id && x.UserId == uid);
            if (r == null) return NotFound("Resume not found");

            r.Title = dto.Title;
            r.PersonalInfo = dto.PersonalInfo;
            r.Education = dto.Education;
            r.Experience = dto.Experience;
            r.Skills = dto.Skills;
            r.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            return Ok(r);
        }

        // DELETE: api/resumes/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var uid = GetUserId();
            var r = await _db.Resumes.FirstOrDefaultAsync(x => x.ResumeId == id && x.UserId == uid);
            if (r == null) return NotFound("Resume not found");

            _db.Resumes.Remove(r);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // AI Suggestions Endpoint
        [HttpPost("suggestions/{id:int}")]
        public async Task<IActionResult> GetSuggestions(int id)
        {
            var uid = GetUserId();
            var resume = await _db.Resumes.FirstOrDefaultAsync(r => r.ResumeId == id && r.UserId == uid);
            if (resume == null) return NotFound("Resume not found");

            var suggestions = new List<string>();

            if (string.IsNullOrWhiteSpace(resume.Skills))
                suggestions.Add("Consider adding more relevant skills for your industry.");

            if (string.IsNullOrWhiteSpace(resume.Experience))
                suggestions.Add("Include professional experience to strengthen your resume.");

            if (resume.PersonalInfo?.Length < 20)
                suggestions.Add("Expand your personal summary to better highlight your strengths.");

            if (resume.Skills.Split(',').Length < 5)
                suggestions.Add("Consider adding more technical skills to make your profile stronger.");

            if (!resume.Experience.ToLower().Contains("project"))
                suggestions.Add("Add some key projects to highlight practical experience.");

            if (resume.PersonalInfo?.Length < 40)
                suggestions.Add("Expand your personal info with address, email, and LinkedIn profile.");

            if (!suggestions.Any())
                suggestions.Add("Your resume looks good! Consider tailoring it for specific job roles.");

            // Save suggestions in DB (as JSON string)
            resume.AiSuggestions = JsonSerializer.Serialize(suggestions);
            resume.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            return Ok(new { Suggestions = suggestions });
        }
    }
}
