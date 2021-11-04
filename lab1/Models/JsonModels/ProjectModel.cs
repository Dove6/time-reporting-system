using System.Collections.Generic;

namespace TRS.Models.JsonModels
{
    public class ProjectModel
    {
        public string Code { get; set; }
        public string Manager { get; set; }
        public string Name { get; set; }
        public int Budget { get; set; }
        public bool Active { get; set; }
        public List<DomainModels.CategoryModel> Subactivities { get; set; }
    }
}
