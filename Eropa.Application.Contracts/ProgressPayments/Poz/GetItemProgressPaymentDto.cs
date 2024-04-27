using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.ProgressPayments
{
    public class GetItemProgressPaymentDto
    {
        [Key]
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
    }
}
