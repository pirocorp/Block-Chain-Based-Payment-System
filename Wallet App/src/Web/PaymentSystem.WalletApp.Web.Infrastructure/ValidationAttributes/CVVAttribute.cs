namespace PaymentSystem.WalletApp.Web.Infrastructure.ValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    [AttributeUsage(
        AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class CVVAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not string cvv)
            {
                return false;
            }

            if (cvv.Any(c => !char.IsDigit(c)))
            {
                return false;
            }

            if (cvv.Length != 3 && cvv.Length != 4)
            {
                return false;
            }

            return true;
        }
    }
}
