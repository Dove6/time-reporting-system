using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trs.Models.DomainModels;

public class Category
{
    public string Code { get; set; } = "";

    public string ProjectCode { get; set; } = "";

    [ForeignKey(nameof(ProjectCode))]
    public Project? Project { get; set; }
}
