using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeApi.Models
{
    public class AspNetUserLogins
    {
        [Key, Column(Order = 0)]
        public string LoginProvider { get; set; } = null!;

        //[Key, Column(Order = 1)]
        public string ProviderKey { get; set; } = null!;

        public string? ProviderDisplayName { get; set; }

        public string UserId { get; set; } = null!;

        // Navigation property
        public virtual AspNetUsers User { get; set; } = null!;
    }
}