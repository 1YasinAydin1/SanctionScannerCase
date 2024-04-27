using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.ProgressPayments
{
    public class SelectPozDto
    {
        [Key]
        public int LineNum { get; set; }
        public string PozLineNum { get; set; }
        public int budgetId { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string activityCode { get; set; }
        public string activityName { get; set; }
        public string kisim { get; set; }
        public double Quantity { get; set; }
        public string Currency { get; set; }
        public double Price { get; set; }
        public string VatGroup { get; set; }
    }
}
