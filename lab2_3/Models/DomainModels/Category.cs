using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trs.Models.DomainModels;

public class Category
{
    public string Code { get; set; } = "";

    [ForeignKey(nameof(Project))]
    public string ProjectCode { get; set; } = "";

    public Project? Project { get; set; }

    [Timestamp]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public byte[] Timestamp { get; set; }
}
