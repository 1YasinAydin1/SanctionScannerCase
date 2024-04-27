using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.Budgets
{
    public class GetMeasurementUnitDto
    {
        [Key]
        public string UomCode { get; set; }
        public string UomName { get; set; }
    }
}
