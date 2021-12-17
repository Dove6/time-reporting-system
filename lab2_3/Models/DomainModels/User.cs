using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Trs.Models.DomainModels;

[Index(nameof(Name), IsUnique = true)]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public ICollection<Report>? Reports { get; set; }
    public ICollection<Project>? Projects { get; set; }

    [Timestamp]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public byte[] Timestamp { get; set; }
}
