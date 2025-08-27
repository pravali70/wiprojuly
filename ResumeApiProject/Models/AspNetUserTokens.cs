using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeApi.Models
{
    public class AspNetUserTokens
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; } = null!;

        //[Key, Column(Order = 1)]
        public string LoginProvider { get; set; } = null!;

        //[Key, Column(Order = 2)]
        public string Name { get; set; } = null!;

        public string? Value { get; set; }

        // Navigation property
        public virtual AspNetUsers User { get; set; } = null!;
    }
}