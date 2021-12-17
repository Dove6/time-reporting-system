using System.ComponentModel.DataAnnotations.Schema;

namespace Trs.Models.DomainModels;

public class Category
{
    public string Code { get; set; } = "";

    [ForeignKey(nameof(Project))]
    public string ProjectCode { get; set; } = "";

    public Project? Project { get; set; }
}
