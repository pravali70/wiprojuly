using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeApi.Models
{
    public class AspNetUserRoles
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; } = null!;

        //[Key, Column(Order = 1)]
        public string RoleId { get; set; } = null!;

        // Navigation properties
        public virtual AspNetUsers User { get; set; } = null!;
        public virtual AspNetRoles Role { get; set; } = null!;
    }
}