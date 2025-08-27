namespace ResumeApi.DTOs
{
    public class ResumeDto
    {
        public string UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PersonalInfo { get; set; } // JSON or text (your schema)
        public string? Education { get; set; }
        public string? Experience { get; set; }
        public string? Skills { get; set; }       // comma-separated or JSON
    }
}