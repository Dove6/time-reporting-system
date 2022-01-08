namespace Trs.Models.RestModels;

public class ProjectListResponseEntry
{
    public string Code { get; set; }
    public string Name { get; set; }
    public List<CategoryModel> Subcategories { get; set; }
}
