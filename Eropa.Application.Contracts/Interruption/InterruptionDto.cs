using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.Interruption
{
    public class InterruptionDto
    {
        [Key]
        public string Code { get; set; }
        public int rowNum { get; set; }
        public string U_KesintiKodu { get; set; }
        public string? U_KesintiTanimi { get; set; }
        public string? U_HesapKodu { get; set; }
        public int? U_TutanakTipi { get; set; }
        public double? U_Oran { get; set; }
        public int? U_faturalanackmi { get; set; }
    }
}
