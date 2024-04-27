using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.ProgressPayments
{
    public class GetPozCurrencyDto
    {
        [Key]
        public string CurrCode { get; set; }
        public string CurrName { get; set; }
    }
}
