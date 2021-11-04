using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class AcceptedSummaryModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public int Time { get; set; }
    }
}
