using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.Budgets
{
    public class GetCurrencyDto
    {
        [Key]
        public string CurrCode { get; set; }
        public string CurrName { get; set; }
    }
}
