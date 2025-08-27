using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResumeApi.Models
{
    public class AspNetRoles
    {
        [Key]
        public string Id { get; set; } = null!;

        [MaxLength(256)]
        public string? Name { get; set; }

        [MaxLength(256)]
        public string? NormalizedName { get; set; }

        public string? ConcurrencyStamp { get; set; }

        // Navigation properties
        public virtual ICollection<AspNetUserRoles> UserRoles { get; set; } = new List<AspNetUserRoles>();
    }
}