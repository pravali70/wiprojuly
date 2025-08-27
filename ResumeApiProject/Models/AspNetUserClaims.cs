using System.ComponentModel.DataAnnotations;

namespace ResumeApi.Models
{
    public class AspNetUserClaims
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }

        // Navigation property
        public virtual AspNetUsers User { get; set; } = null!;
    }
}