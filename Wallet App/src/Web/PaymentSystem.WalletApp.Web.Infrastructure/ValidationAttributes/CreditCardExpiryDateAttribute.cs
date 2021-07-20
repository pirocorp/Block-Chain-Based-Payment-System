namespace PaymentSystem.WalletApp.Web.Infrastructure.ValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(
        AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class CreditCardExpiryDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is not string expiryDate)
            {
                return false;
            }

            if (expiryDate.Length != 5)
            {
                return false;
            }

            var parts = expiryDate.Split("/");

            var month = int.Parse(parts[0]);
            var year = int.Parse(parts[1]);

            if (month < 1 || month > 12)
            {
                return false;
            }

            if (year < 0 || year > 99)
            {
                return false;
            }

            return true;
        }
    }
}
