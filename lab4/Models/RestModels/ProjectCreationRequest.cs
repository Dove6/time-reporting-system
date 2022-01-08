using System.ComponentModel.DataAnnotations;

namespace Trs.Models.RestModels;

public class ProjectCreationRequest
{
    public string Name { get; set; }
    [Range(0, int.MaxValue)]
    public int Budget { get; set; }
    public List<CategoryModel> Categories { get; set; }
}
