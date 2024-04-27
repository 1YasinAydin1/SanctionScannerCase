using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.Interruption
{
    public class AccountCodeDto
    {
        [Key]
        public string AcctCode { get; set; }
        public string AcctName { get; set; }
    }
}
