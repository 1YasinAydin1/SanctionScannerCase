using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.Budgets
{
    public class ProjectDto
    {
        [Key]
        public string PrjCode { get; set; }
        public string PrjName { get; set; }
    }
}
