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
    //[Authorize] // default: requires auth
    public class ResumesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ResumesController(AppDbContext db)
        {
            _db = db;
        }

        // Identity UserId is string (GUID), not int
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue("sub")!;
        }

        private bool IsAdmin() => User.IsInRole("Admin");


        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            var list = await _db.Resumes.ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _db.Resumes.FirstOrDefaultAsync(x => x.ResumeId == id);

            if (r == null) return NotFound();
            return Ok(r);
        }

        // Download Resume as PDF
        [HttpGet("download/{id:int}")]
        //[Authorize(Roles = "RegisteredUser,Admin")]
        public async Task<IActionResult> DownloadResume(int id)
        {
            var resume = await _db.Resumes.FirstOrDefaultAsync(r => r.ResumeId == id);
            if (resume == null) return NotFound("Resume not found");

            //if (!IsAdmin() && resume.UserId != GetUserId()) // ✅ string check
            //return Forbid();

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

        [HttpPost]
        public async Task<IActionResult> Create(ResumeDto dto)
        {
            //var uid = GetUserId(); //  string
            //Console.WriteLine(uid);
            var r = new Resume
            {
                UserId = dto.UserId,   //  assign string
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
            var r = await _db.Resumes.FirstOrDefaultAsync(x => x.ResumeId == id);
            if (r == null) return NotFound();

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
            var r = await _db.Resumes.FirstOrDefaultAsync(x => x.ResumeId == id);
            if (r == null) return NotFound();

            _db.Resumes.Remove(r);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        //  AI Suggestions Endpoint
        [HttpPost("suggestions/{id:int}")]
        //[Authorize(Roles = "RegisteredUser,Admin")]
        public async Task<IActionResult> GetSuggestions(int id)
        {
            var resume = await _db.Resumes.FirstOrDefaultAsync(r => r.ResumeId == id);
            if (resume == null) return NotFound("Resume not found");

            //if (!IsAdmin() && resume.UserId != GetUserId()) // string check
            //return Forbid();

            var suggestions = new List<string>();

            if (string.IsNullOrWhiteSpace(resume.Skills))
                suggestions.Add("Consider adding more relevant skills for your industry.");

            if (string.IsNullOrWhiteSpace(resume.Experience))
                suggestions.Add("Include professional experience to strengthen your resume.");

            if (resume.PersonalInfo?.Length < 20)
                suggestions.Add("Expand your personal summary to better highlight your strengths.");

            // Save suggestions in DB (as JSON string)
            resume.AiSuggestions = JsonSerializer.Serialize(suggestions);
            resume.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            return Ok(new { Suggestions = suggestions });
        }
    }
}