using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.ProgressPayments
{
    public class GetVatgroupDto
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
