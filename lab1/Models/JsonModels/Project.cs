using System.Collections.Generic;

namespace TRS.Models.JsonModels
{
    public class Project
    {
        public string Code { get; set; }
        public string Manager { get; set; }
        public string Name { get; set; }
        public int Budget { get; set; }
        public bool Active { get; set; }
        public List<DomainModels.Category> Subactivities { get; set; }
    }
}
