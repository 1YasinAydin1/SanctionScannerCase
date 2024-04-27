using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.Budgets
{
    public class CurrencyCurrDto
    {
        [Key]
        public string Currency { get; set; }
        public double Rate { get; set; }
    }
}
