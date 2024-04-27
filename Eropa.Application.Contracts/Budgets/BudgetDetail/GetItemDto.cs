using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.Budgets
{
    public class GetItemDto
    {
        [Key]
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
    }
}
