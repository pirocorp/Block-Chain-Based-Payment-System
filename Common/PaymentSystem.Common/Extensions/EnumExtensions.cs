namespace PaymentSystem.Common.Extensions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    public static class EnumExtensions
    {
        public static string GetDisplayNameValue(this Enum e)
            => e
                .GetType()
                .GetMember(e.ToString())
                .FirstOrDefault()
                ?.GetCustomAttribute<DisplayAttribute>()
                ?.GetName() ?? string.Empty;
    }
}
