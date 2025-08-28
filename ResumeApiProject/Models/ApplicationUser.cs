using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ResumeApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        // 👇 Add UserType for role management
        public string UserType { get; set; } = "RegisteredUser";

        // Navigation property to Resumes
        public virtual ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    }
}
