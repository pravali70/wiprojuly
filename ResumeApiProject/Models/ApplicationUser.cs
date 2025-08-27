using Microsoft.AspNetCore.Identity;

namespace ResumeApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        //  Navigation property to Resumes
        public virtual ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    }
}