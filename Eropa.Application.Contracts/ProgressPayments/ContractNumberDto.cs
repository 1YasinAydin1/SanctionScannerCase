using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.ProgressPayments
{
    public class ContractNumberDto
    {
        // SELECT A. "DocDate" AS "Kayıt Tarihi" , A. "NumAtCard" AS "Müş.Ref.No" , A. "DocNum" AS "Sözleşme No" , A. "U_btcKonu" AS "İşin Konusu" , MAX ( B. "U_btcBelgeNo" ) AS "Bütçe ID" FROM "OPOR" A , "POR1" B WHERE A. "DocEntry" = B. "DocEntry" AND A. "CANCELED" = N'N' AND A. "DocStatus" = N'O' GROUP BY A. "DocDate" , A. "NumAtCard" , A. "DocNum" , A. "U_btcKonu"
        [Key]
        public int DocNum { get; set; }
        public DateTime? DocDate { get; set; }
        public string? NumAtCard { get; set; }
        public string? U_btcKonu { get; set; }
  
        public int? budgetID { get; set; }
    }
}
