using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.ProgressPayments
{
    public class WithholdingTaxDto
    {
        [Key]
        public string WTCode { get; set; }
        public string WTName { get; set; }
    }
}
