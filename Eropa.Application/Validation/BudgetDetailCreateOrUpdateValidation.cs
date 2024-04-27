using Eropa.Application.Contracts.Budgets;
using FluentValidation;

namespace Eropa.Application.Validation
{
    public class BudgetDetailCreateOrUpdateValidation : AbstractValidator<BudgetDetailCreateOrUpdateDto>
    {
        public BudgetDetailCreateOrUpdateValidation()
        {
            RuleFor(x => x.U_AktiviteKodu).NotNull().WithMessage("Aktivite Kodu Alanı Boş Geçilemez.");

            //When(x => x.U_AktiviteKodu.Length==6, () =>
            //{
            //    RuleFor(x => x.U_AktiviteKodu).NotNull().WithMessage("Aktivite Kodu Alanı Boş Geçilemez.");
            //    //RuleFor(x => x.U_KalemKodu).NotNull().WithMessage("Kalem Kodu Alanı Boş Geçilemez.");
            //    //RuleFor(x => x.U_Birim).NotNull().WithMessage("Birim Alanı Boş Geçilemez.");
            //    //RuleFor(x => x.U_ParaBirimi).NotNull().WithMessage("Para Birimi Alanı Boş Geçilemez.");
            //    //RuleFor(x => x.U_Kur).NotNull().WithMessage("Kur Alanı Boş Geçilemez.").GreaterThan(0).WithMessage("Kur Alanı 0' dan Büyük Olmalıdır.");
            //    //RuleFor(x => x.U_BirimFiyat).NotNull().WithMessage("Birim Fiyat Alanı Boş Geçilemez.").GreaterThan(0).WithMessage("Birim Fiyat Alanı 0' dan Büyük Olmalıdır.");
            //    //RuleFor(x => x.U_Miktar).NotNull().WithMessage("Miktar Alanı Boş Geçilemez.").GreaterThan(0).WithMessage("Miktar Alanı 0' dan Büyük Olmalıdır.");
            //});
            //When(x => x.U_AktiviteKodu.Length == 2 || x.U_AktiviteKodu.Length == 4, () =>
            //{
            //    RuleFor(x => x.U_AktiviteKodu).NotNull().WithMessage("Aktivite Kodu Alanı Boş Geçilemez.");
            //});
        }
    }
}
