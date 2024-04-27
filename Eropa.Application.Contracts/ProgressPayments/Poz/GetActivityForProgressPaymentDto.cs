using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.ProgressPayments
{
    public class GetActivityForProgressPaymentDto
    {
        [Key]
        public string UniqueCode { get; set; }
        public string ActivityCode { get; set; }
        public string ActivityName { get; set; }
        public string Kisim { get; set; }
    }
}
