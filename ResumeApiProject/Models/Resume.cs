using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ResumeApi.Models;

public class Resume
{
    [Key]
    public int ResumeId { get; set; }

    //  FK to IdentityUser (string Id)
    public string? UserId { get; set; }
    // Foreign Key


    // Navigation Property
    //[ForeignKey("UserId")]

    //public AspNetUsers AspNetUsers { get; set; }

    [StringLength(100)]
    public string Title { get; set; } = null!;

    public string? PersonalInfo { get; set; }
    public string? Education { get; set; }
    public string? Experience { get; set; }
    public string? Skills { get; set; }
    public string? AiSuggestions { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }
}