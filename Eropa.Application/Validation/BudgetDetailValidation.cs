using Eropa.Application.Contracts.Budgets;
using FluentValidation;

namespace Eropa.Application.Validation
{
    public class BudgetDetailValidation : AbstractValidator<BudgetDetailDto>
    {
        public BudgetDetailValidation()
        {
                RuleFor(x => x.U_AktiviteKodu).NotNull().WithMessage("Aktivite Kodu Alanı Boş Geçilemez.");
        //    When(x => !string.IsNullOrEmpty(x.U_Seviye) ? x.U_Seviye.Equals("3") : true, () =>
        //    {
        //        RuleFor(x => x.U_AktiviteKodu).NotNull().WithMessage("Aktivite Kodu Alanı Boş Geçilemez.");
        //        //RuleFor(x => x.U_KalemKodu).NotNull().WithMessage("Kalem Kodu Alanı Boş Geçilemez.");
        //        //RuleFor(x => x.U_Birim).NotNull().WithMessage("Birim Alanı Boş Geçilemez.");
        //        //RuleFor(x => x.U_ParaBirimi).NotNull().WithMessage("Para Birimi Alanı Boş Geçilemez.");
        //        //RuleFor(x => x.U_Kur).NotNull().WithMessage("Kur Alanı Boş Geçilemez.").GreaterThan(0).WithMessage("Kur Alanı 0' dan Büyük Olmalıdır.");
        //        //RuleFor(x => x.U_BirimFiyat).NotNull().WithMessage("Birim Fiyat Alanı Boş Geçilemez.").GreaterThan(0).WithMessage("Birim Fiyat Alanı 0' dan Büyük Olmalıdır.");
        //        //RuleFor(x => x.U_Miktar).NotNull().WithMessage("Miktar Alanı Boş Geçilemez.").GreaterThan(0).WithMessage("Miktar Alanı 0' dan Büyük Olmalıdır.");
        //    });
        //    When(x => !string.IsNullOrEmpty(x.U_Seviye) ? x.U_Seviye.Equals("1") || x.U_Seviye.Equals("2") : true, () =>
        //    {
        //        RuleFor(x => x.U_AktiviteKodu).NotNull().WithMessage("Aktivite Kodu Alanı Boş Geçilemez.");
        //    });
        //}
    }
    }
}
