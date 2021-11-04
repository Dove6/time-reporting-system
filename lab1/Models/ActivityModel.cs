using System.Collections.Generic;

namespace TRS.Models
{
    public class ActivityModel
    {
        public string Code { get; set; }
        public string Manager { get; set; }
        public string Name { get; set; }
        public int Budget { get; set; }
        public bool Active { get; set; }
        public List<SubactivityModel> Subactivities { get; set; }
    }
}
