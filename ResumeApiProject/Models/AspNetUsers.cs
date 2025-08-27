using System;
using System.Collections.Generic;

namespace ResumeApi.Models
{
    public class AspNetUsers
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; } = false;
        public bool TwoFactorEnabled { get; set; } = false;
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; } = true;
        public int AccessFailedCount { get; set; } = 0;

        // 🔗 Navigation properties
        public ICollection<AspNetUserRoles> UserRoles { get; set; } = new List<AspNetUserRoles>();
        public ICollection<AspNetUserClaims> UserClaims { get; set; } = new List<AspNetUserClaims>();
        public ICollection<AspNetUserLogins> UserLogins { get; set; } = new List<AspNetUserLogins>();
        public ICollection<AspNetUserTokens> UserTokens { get; set; } = new List<AspNetUserTokens>();
        public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    }
}