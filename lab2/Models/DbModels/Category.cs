using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Trs.Models.DbModels;

[Index(nameof(ProjectCode), nameof(Name), IsUnique = true)]
public class Category
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = "";

    [ForeignKey(nameof(Project))]
    public string ProjectCode { get; set; } = "";
    public virtual Project? Project { get; set; }
}
